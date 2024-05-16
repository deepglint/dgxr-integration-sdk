using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deepglint.Tool.Manager
{
    public enum AudioType
    {
        // 背景音乐
        BGM,

        // 音效
        SoundEffect,

        // 人声旁白
        Voice
    }

    public static class AudioManager
    {
        public class Audio
        {
            public AudioType Type;
            public AudioSource Source;
            public string Name;
        }

        public class AudioList
        {
            public string Name;
            public List<Audio> Audios;
        }

        private const string Path = "Audio/";
        private static List<Audio> _audios = new List<Audio>();
        private static List<AudioList> _audioLists = new List<AudioList>();
        private static GameObject _audioRoot;


        /// <summary>
        /// 加载音频
        /// </summary>
        /// <param name="audioName">音频名称</param>
        /// <param name="audioType">音频类型</param>
        /// <returns>是否成功在指定文件夹下Load传入音频</returns>
        private static Audio CreateAudio(string audioName)
        {
            if (_audioRoot == null)
            {
                _audioRoot = new GameObject("AudioRoot");
                Object.DontDestroyOnLoad(_audioRoot);
            }

            var audioClip = Resources.Load<AudioClip>(Path + audioName);
            if (audioClip == null) return null;
            var obj = new GameObject(audioClip.name);
            obj.transform.SetParent(_audioRoot.transform);
            var source = obj.AddComponent<AudioSource>();
            source.clip = audioClip;
            Audio audio = new Audio()
            {
                Name = audioName,
                Source = source,
            };
            return audio;
        }

        private static Audio FindAudio(string name, AudioType type = 0)
        {
            if (type != 0)
            {
                return _audios.FirstOrDefault(audio => audio.Name == name && audio.Type == type);
            }

            return _audios.FirstOrDefault(audio => audio.Name == name);
        }

        private static AudioList FindAudioList(string audioListName)
        {
            return _audioLists.FirstOrDefault(audioList => audioList.Name == audioListName);
        }


        private static List<Audio> FindAudiosByType(AudioType[] types)
        {
            return _audios.Where(audio => types.Contains(audio.Type)).ToList();
        }

        /// <summary>
        /// 获取音频，如果是第一次获取会创建，否则拿存储
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Audio GetAudio(string audioName, AudioType type = AudioType.Voice)
        {
            Audio audio = FindAudio(audioName, type);
            if (audio is null)
            {
                Audio newAudio = CreateAudio(audioName);
                if (newAudio != null)
                {
                    newAudio.Type = type;
                    _audios.Add(newAudio);
                    audio = newAudio;
                }
                else
                {
                    Debug.LogError("AudioManager " + type + " - " + audioName + " 音频不存在");
                }
            }

            return audio;
        }

        /// <summary>
        /// 获取音频长度
        /// </summary>
        /// <param name="name"></param>
        /// <returns>视频长度，单位毫秒</returns>
        public static float GetAudioLength(string name)
        {
            Audio audio = FindAudio(name);
            if (audio != null)
            {
                return audio.Source.clip.length * 1000f;
            }

            Debug.LogError("AudioManager " + name + " 音频不存在");
            return 0;
        }

        public static float GetAudioLength(AudioSource source)
        {
            return source.clip.length * 1000f;
        }

        /// <summary>
        /// 播放指定音频
        /// </summary>
        /// <param name="audioName">音频名称</param>
        /// <param name="type">音频类型</param>
        /// <param name="iswait"></param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="volume">音量</param>
        public static void PlayAudio(string audioName, AudioType type, bool iswait = false, bool loop = false,
            float volume = 1)
        {
            Audio audio = GetAudio(audioName, type);
            if (audio is null)
            {
                return;
            }

            if (iswait)
            {
                if (!audio.Source.isPlaying)
                {
                    audio.Source.Play();
                    audio.Source.volume = volume;
                    audio.Source.loop = loop;
                }
            }
            else
            {
                audio.Source.Play();
                audio.Source.volume = volume;
                audio.Source.loop = loop;
            }
        }

        public static async void PlayAudioList(List<string> audioNameList, string audioListName, float volume = 1)
        {
            List<Audio> audioList = new List<Audio>();
            foreach (var audioName in audioNameList)
            {
                Audio audio = GetAudio(audioName);
                if (audio != null)
                {
                    audioList.Add(audio);
                }
            }

            AudioList list = new AudioList()
            {
                Name = audioListName,
                Audios = audioList
            };
            _audioLists.Add(list);
            foreach (var audio in list.Audios)
            {
                audio.Source.Play();
                audio.Source.volume = volume;
                await Task.Delay(TimeSpan.FromMilliseconds(GetAudioLength(audio.Source)));
                if (list.Audios.Count == 0)
                {
                    break;
                }
            }

            _audioLists.RemoveAll(item => item.Name == audioListName);
        }

        public static void StopAudioList(string audioListName)
        {
            AudioList list = FindAudioList(audioListName);
            if (list != null)
            {
                foreach (var listAudio in list.Audios)
                {
                    listAudio.Source.Stop();
                }

                list.Audios.Clear();

                _audioLists.RemoveAll(item => item.Name == audioListName);
            }
        }

        public static float GetAudioListLength(string audioListName)
        {
            AudioList list = FindAudioList(audioListName);
            float length = 0;
            if (list != null)
            {
                foreach (var listAudio in list.Audios)
                {
                    length += listAudio.Source.clip.length * 1000f;
                }

                return length;
            }

            Debug.LogError("AudioManager " + audioListName + " 音频列表不存在");
            return 0;
        }


        /// <summary>
        /// 停止指定音频播放
        /// </summary>
        /// <param name="audioName">音频名称</param>
        public static void StopAudio(string audioName)
        {
            Audio audio = FindAudio(audioName);
            if (audio != null)
            {
                audio.Source.Stop();
            }
            else
            {
                Debug.LogWarning("AudioManager " + audioName + " 音频不存在");
            }
        }

        /// <summary>
        /// 停止所有指定类型音频
        /// </summary>
        /// <param name="types">音频类型</param>
        public static void StopAudioByType(AudioType[] types)
        {
            List<Audio> typeAudios = FindAudiosByType(types);
            if (typeAudios.Count > 0)
            {
                foreach (var typeAudio in typeAudios)
                {
                    typeAudio.Source.Stop();
                }
            }
        }

        public static void StopAudioByType(AudioType type)
        {
            AudioType[] types = { type };
            List<Audio> typeAudios = FindAudiosByType(types);
            if (typeAudios.Count > 0)
            {
                foreach (var typeAudio in typeAudios)
                {
                    typeAudio.Source.Stop();
                }
            }
        }
    }
}
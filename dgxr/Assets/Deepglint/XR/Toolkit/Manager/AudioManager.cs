using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deepglint.XR.Toolkit.Manager
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

    public class Audio
    {
        private const string BasePath = "Audio";
        private static GameObject _audioRoot;

        public readonly string Name;
        public readonly AudioType Type;
        public readonly AudioSource Source;
        public float Length => AudioLength(this);

        public Audio(string name, AudioType type = AudioType.SoundEffect)
        {
            Name = name;
            Type = type;
            Source = CreateAudioSource(Name);
        }

        public void Play(bool ignoreIfPlaying = false, bool loop = false, float volume = 1)
        {
            AudioManager.PlayAudio(this, ignoreIfPlaying, loop, volume);
        }


        /// <summary>
        /// 获取音频长度
        /// </summary>
        /// <param name="audio">音频</param>
        /// <returns>视频长度，单位毫秒</returns>
        public static float AudioLength(Audio audio)
        {
            if (audio == null)
            {
                throw new NullReferenceException("audio is null");
            }

            return audio.Source.clip.length * 1000f;
        }


        /// <summary>
        /// 加载音频
        /// </summary>
        /// <param name="audioName">音频名称</param>
        /// <returns>是否成功在指定文件夹下Load传入音频</returns>
        private static AudioSource CreateAudioSource(string audioName)
        {
            if (_audioRoot == null)
            {
                _audioRoot = new GameObject("AudioRoot");
                Object.DontDestroyOnLoad(_audioRoot);
            }

            var audioClip = Resources.Load<AudioClip>(Path.Combine(BasePath, audioName));
            if (audioClip == null) return null;
            var obj = new GameObject(audioClip.name);
            obj.transform.SetParent(_audioRoot.transform);
            var source = obj.AddComponent<AudioSource>();
            source.clip = audioClip;
            return source;
        }
    }

    public class AudioList
    {
        public string Name;
        public List<Audio> Audios;
    }

    public static class AudioManager
    {
        private static readonly List<Audio> Audios = new();
        private static readonly List<AudioList> AudioLists = new();

        private static Audio FindAudio(Audio audio)
        {
            return Audios.FirstOrDefault(item => item == audio);
        }

        private static AudioList FindAudioList(string audioListName)
        {
            return AudioLists.FirstOrDefault(audioList => audioList.Name == audioListName);
        }


        private static List<Audio> FindAudiosByType(AudioType[] types)
        {
            return Audios.Where(audio => types.Contains(audio.Type)).ToList();
        }

        /// <summary>
        /// 播放指定音频
        /// </summary>
        /// <param name="audio">音频</param>
        /// <param name="ignoreIfPlaying">是否等待播放完</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="volume">音量</param>
        public static void PlayAudio(Audio audio, bool ignoreIfPlaying = false, bool loop = false,
            float volume = 1)
        {
            if (audio == null)
            {
                throw new NullReferenceException();
            }

            if (FindAudio(audio) != null)
            {
                Audios.Add(audio);
            }

            if (ignoreIfPlaying)
            {
                if (audio.Source.isPlaying) return;
            }
            else
            {
                audio.Source.Play();
            }

            audio.Source.volume = volume;
            audio.Source.loop = loop;
        }


        public static async void PlayAudioList(AudioList list, string audioListName, float volume = 1)
        {
            AudioLists.Add(list);
            foreach (var audio in list.Audios)
            {
                audio.Source.Play();
                audio.Source.volume = volume;
                await Task.Delay(TimeSpan.FromMilliseconds(audio.Length));
                if (list.Audios.Count == 0)
                {
                    break;
                }
            }

            AudioLists.RemoveAll(item => item.Name == audioListName);
        }

        public static void StopAudioListByName(string audioListName)
        {
            var list = FindAudioList(audioListName);
            StopAudioList(list);
        }

        private static void StopAudioList(AudioList list)
        {
            if (list == null) return;
            foreach (var listAudio in list.Audios)
            {
                listAudio.Source.Stop();
            }

            list.Audios.Clear();
            AudioLists.RemoveAll(item => item.Name == list.Name);
        }

        public static float GetAudioListLength(string audioListName)
        {
            var list = FindAudioList(audioListName);
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
        /// <param name="audio">音频</param>
        public static void StopAudio(Audio audio)
        {
            if (audio == null)
            {
                throw new NullReferenceException("audio is null");
            }

            audio.Source.Stop();
        }

        /// <summary>
        /// 停止所有指定类型音频
        /// </summary>
        /// <param name="types">音频类型</param>
        public static void StopAudioByType(AudioType[] types)
        {
            var typeAudios = FindAudiosByType(types);
            if (typeAudios.Count <= 0) return;
            foreach (var typeAudio in typeAudios)
            {
                typeAudio.Source.Stop();
            }
        }

        public static void StopAudioByType(AudioType type)
        {
            AudioType[] types = { type };
            var typeAudios = FindAudiosByType(types);
            if (typeAudios.Count <= 0) return;
            foreach (var typeAudio in typeAudios)
            {
                typeAudio.Source.Stop();
            }
        }


        /// <summary>
        /// 停止所有音频
        /// </summary>
        public static void StopAll()
        {
            foreach (var typeAudio in Audios)
            {
                typeAudio.Source.Stop();
            }
        }
    }
}

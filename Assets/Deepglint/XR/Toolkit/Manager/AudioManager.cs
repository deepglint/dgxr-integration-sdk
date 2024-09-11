using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deepglint.XR.Toolkit.Manager
{
    /// <summary>
    /// 音频类型
    /// </summary>
    public enum AudioType
    {
        /// <summary>
        /// 背景音乐
        /// </summary>
        BGM,

        /// <summary>
        /// 音效
        /// </summary>
        SoundEffect,

        /// <summary>
        /// 人声旁白
        /// </summary>
        Voice
    }

    /// <summary>
    /// 音频类，用于加载，播放音频
    /// 音频资源从 `Assets/Resources/Audio` 目录下加载
    /// </summary>
    public class Audio
    {
        private const string BasePath = "Audios";
        private static GameObject _audioRoot;

        /// <summary>
        /// 加载的音频资源名称
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 音频类型
        /// </summary>
        public readonly AudioType Type;

        /// <summary>
        /// 实际播放时使用的 Unity AudioSource
        /// </summary>
        public readonly AudioSource Source;

        /// <summary>
        /// 音频时长，单位：毫秒
        /// </summary>
        public float Length => AudioLength(this);

        /// <summary>
        /// 创建音频对象
        /// </summary>
        /// <param name="name">音频资源名称</param>
        /// <param name="type">音频类型</param>
        public Audio(string name, AudioType type = AudioType.SoundEffect)
        {
            Name = name;
            Type = type;
            Source = CreateAudioSource(Name, Type);
            AudioManager.Audios.Add(this);
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="ignoreIfPlaying">如果正在播放，是否重复播放</param>
        /// <param name="loop">是否循环播放, 背景音乐不设置loop为true也会循环播放</param>
        /// <param name="volume">播放音量</param>
        public void Play(bool ignoreIfPlaying = false, bool loop = false, float volume = 1)
        {
            if (Type == AudioType.BGM)
            {
                loop = true;
            }
            
            AudioManager.PlayAudio(this, ignoreIfPlaying, loop, volume);
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            AudioManager.StopAudio(this);
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
        /// <param name="audioType">音频类型</param>
        /// <returns>是否成功在指定文件夹下Load传入音频</returns>
        private static AudioSource CreateAudioSource(string audioName, AudioType audioType)
        {
            if (_audioRoot == null)
            {
                _audioRoot = new GameObject("AudioRoot");
                _audioRoot.transform.parent = XRManager.XRDontDestroy.transform;
            }

            var audioClip = Resources.Load<AudioClip>(Path.Combine(BasePath, audioType.ToString(), audioName));
            if (audioClip == null)
            {
                throw new FileNotFoundException(
                    $"audio file {Path.Combine(BasePath, audioType.ToString(), audioName)} not found");
            }

            var obj = new GameObject(audioClip.name);
            obj.transform.SetParent(_audioRoot.transform);
            var source = obj.AddComponent<AudioSource>();
            source.clip = audioClip;
            return source;
        }
    }

    /// <summary>
    /// 音频列表，列表中的音频可以按序播放
    /// </summary>
    public class AudioList
    {
        /// <summary>
        /// 列表名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 包含的音频对象
        /// </summary>
        public List<Audio> Audios { get; private set; }

        /// <summary>
        /// 创建空音频列表
        /// </summary>
        /// <param name="name">名称</param>
        public AudioList(string name)
        {
            Name = name;
            Audios = new List<Audio>();
        }

        /// <summary>
        /// 创建音频列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="list">音频对象列表</param>
        public AudioList(string name, List<Audio> list)
        {
            Name = name;
            Audios = list;
        }

        /// <summary>
        /// 添加音频对象
        /// </summary>
        /// <param name="audio"></param>
        public void Add(Audio audio)
        {
            Audios.Add(audio);
        }

        /// <summary>
        /// 播放音频列表
        /// </summary>
        public void Play()
        {
            AudioManager.PlayAudioList(this);
        }
    }

    /// <summary>
    /// 音频管理模块
    /// </summary>
    public static class AudioManager
    {
        public static readonly List<Audio> Audios = new();
        private static readonly List<AudioList> AudioLists = new();

        /// <summary>
        /// 查找音频
        /// </summary>
        /// <param name="audioName">音频名称</param>
        /// <returns>音频对象</returns>
        public static Audio FindAudio(string audioName)
        {
            return Audios.FirstOrDefault(item => item.Name == audioName);
        }

        /// <summary>
        /// 查找音频列表
        /// </summary>
        /// <param name="audioListName">列表名称</param>
        /// <returns>音频列表</returns>
        public static AudioList FindAudioList(string audioListName)
        {
            return AudioLists.FirstOrDefault(audioList => audioList.Name == audioListName);
        }

        /// <summary>
        /// 通过音频类型查找音频
        /// </summary>
        /// <param name="type">音频类型</param>
        /// <returns>音频对象数组</returns>
        public static List<Audio> FindAudiosByType(AudioType type)
        {
            return Audios.Where(audio => audio.Type == type).ToList();
        }

        /// <summary>
        /// 通过音频类型数组查找音频
        /// </summary>
        /// <param name="types">音频类型数组</param>
        /// <returns>音频对象数组</returns>
        public static List<Audio> FindAudiosByType(AudioType[] types)
        {
            return Audios.Where(audio => types.Contains(audio.Type)).ToList();
        }


        /// <summary>
        /// 播放指定音频
        /// </summary>
        /// <param name="audioName">音频名称</param>
        /// <param name="ignoreIfPlaying">是否等待播放完</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="volume">音量</param>
        public static void PlayAudio(string audioName, bool ignoreIfPlaying = false, bool loop = false,
            float volume = 1)
        {
            PlayAudio(FindAudio(audioName), ignoreIfPlaying, loop);
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

            if (ignoreIfPlaying)
            {
                if (audio.Source.isPlaying) return;
            }

            if (audio.Type == AudioType.BGM)
            {
                StopAudioByType(AudioType.BGM);
            }

            audio.Source.Play();
            audio.Source.volume = volume;
            audio.Source.loop = loop;
        }

        /// <summary>
        /// 播放音频列表
        /// </summary>
        /// <param name="list">音频列表</param>
        /// <param name="volume">音量</param>
        public static async void PlayAudioList(AudioList list, float volume = 1)
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

            AudioLists.RemoveAll(item => item.Name == list.Name);
        }


        /// <summary>
        /// 停止音频列表播放
        /// </summary>
        /// <param name="audioListName">音频列表名称</param>
        public static void StopAudioListByName(string audioListName)
        {
            var list = FindAudioList(audioListName);
            StopAudioList(list);
        }

        /// <summary>
        /// 停止音频列表播放
        /// </summary>
        /// <param name="list">音频列表</param>
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

        /// <summary>
        /// 获取音频列表中所有音频长度之和
        /// </summary>
        /// <param name="audioListName">音频列表名称</param>
        /// <returns>音频列表中所有音频长度之和，单位ms</returns>
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

            DGXR.Logger.LogError("AudioManager", "AudioManager " + audioListName + " 音频列表不存在");
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
        /// <param name="types">音频类型，传递多个type拼成的数组</param>
        public static void StopAudioByType(AudioType[] types)
        {
            var typeAudios = FindAudiosByType(types);
            if (typeAudios.Count <= 0) return;
            foreach (var typeAudio in typeAudios)
            {
                typeAudio.Source.Stop();
            }
        }

        /// <summary>
        /// 停止所有指定类型音频
        /// </summary>
        /// <param name="type">音频类型，传递一个type</param>
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

        /// <summary>
        /// 给某类音频设置音量
        /// </summary>
        /// <param name="type">音频类型</param>
        /// <param name="volume">音量</param>
        public static void SetVolumeByType(AudioType type, int volume)
        {
            List<Audio> lists = FindAudiosByType(type);
            if (lists.Count > 0)
            {
                foreach (var audio in lists)
                {
                    audio.Source.volume = volume;
                }
            }
        }
    }
}
using System.Collections.Generic;
using Runtime.Scripts.Tools.Manager;
using UnityEngine;
using AudioType = Runtime.Scripts.Tools.Manager.AudioType;

namespace Runtime.Scripts.Tools.Test
{
    public class AudioTest : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayBGM();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayVoice();
            }
        
            if (Input.GetKeyDown(KeyCode.L))
            {
                PlayVoiceList1();
            }
        
            if (Input.GetKeyDown(KeyCode.I))
            {
                PlayVoiceList2();
            }
        
            if (Input.GetKeyDown(KeyCode.S))
            {
                StopVoice();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                StopVoiceList1();
            }
        
            if (Input.GetKeyDown(KeyCode.O))
            {
                StopVoiceList2();
            }
        }

        void PlayBGM()
        {
            AudioManager.PlayAudio(AudioList.BGM, AudioType.BGM, true);
        }


        private void PlayVoice()
        {
            AudioManager.PlayAudio(AudioList.Voice1, AudioType.Voice, true);
            TimerManager.DoOnce(AudioManager.GetAudioLength(AudioList.Voice1), () =>
            {
                Debug.Log("音频播放结束！");
            });
        }

        void PlayVoiceList1()
        {
            List<string> audios = new List<string>();
            audios.Add(AudioList.Voice1);
            audios.Add(AudioList.Voice2);
            audios.Add(AudioList.Voice3);
            AudioManager.PlayAudioList(audios, "GameOver");
        }
    
        void PlayVoiceList2()
        {
            List<string> audios = new List<string>();
            audios.Add(AudioList.Voice3);
            audios.Add(AudioList.Voice1);
            AudioManager.PlayAudioList(audios, "GameStart");
            TimerManager.DoOnce(AudioManager.GetAudioListLength("GameStart"), () =>
            {
                Debug.Log("音频播放结束！");
            });
        }

        void StopVoice()
        {
            AudioManager.StopAudioByType(AudioType.Voice);
        }

        void StopVoiceList1()
        {
            AudioManager.StopAudioList("GameOver");
        }
        void StopVoiceList2()
        {
            AudioManager.StopAudioList("GameStart");
        }
    }
}

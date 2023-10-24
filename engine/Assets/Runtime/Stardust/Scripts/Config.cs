using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BodySource
{
    public class Config : MonoBehaviour
    {
        public enum ActionType : int
        {
            A = 0,
            B = 1,
            C = 2,
            D = 3,
            Greet = 4, // 招呼
            LeftSlide = 5, // 左边滑
            Squat = 6, // 下蹲
            RightTilt = 7, // 右倾斜
            LeftTilt = 8,// 左倾斜
            ElbowBend = 9, // 弯双肘
            Stand = 10,// 站立
            Jump = 11,//跳跃


            SlowRun = 19, // 慢跑
            FastRun = 20, // 快跑
            HandUp = 10000, // 举手
            CheerUp = 24,// 欢呼
            JumpUp = 25, // 起跳
            SquatDown = 26, // 下蹲
                            // 添加其他可能的动作
        }

        public enum Key : int
        {
            DPadUp = 0,
            DPadDown = 1,
            DPadLeft = 2,
            DPadRight = 3,
            ButtonStart = 4,
            ButtonBack = 5,
            StickLeft = 6,
            StickRight = 7,
            BumperLeft = 8,
            BumperRight = 9,
            ButtonGuide = 10,
            ButtonA = 12,
            ButtonB = 13,
            ButtonX = 14,
            ButtonY = 15,

            LeftStickUp = 16,
            LeftStickDown = 17,
            LeftStickLeft = 18,
            LeftStickRight = 19,
            RightStickUp = 20,
            RightStickDown = 21,
            RightStickLeft = 22,
            RightStickRight = 23,
            LeftTrigger = 24,
            RightTrigger = 25,
            LeftStickZero = 26,
            RightStickZero = 27,
        }

        public enum keyType : int
        {
            Button = 0,
            Stick = 1,
        }

        [System.Serializable]
        public struct Action
        {
            public ActionType action;
            public Key key;
            public keyType type;
        }

        public List<Action> actions;

        private bool isSent = false;

        // Update is called once per frame
        void Update()
        {
            if (!isSent)
            {
                isSent = SentConfig(actions);
            }
        }

        public bool SentConfig(List<Action> conf)
        {
            GameObject source = GameObject.Find("Source");
            Source sourceConnect = source.GetComponent<Source>();
            string jsonString = JsonConvert.SerializeObject(conf);

            if (sourceConnect != null && sourceConnect.webSocket != null)
            {
                Debug.Log(jsonString);
                if (sourceConnect.webSocket.IsOpen)
                {
                    sourceConnect.webSocket.Send(jsonString);
                    return true;
                }
            }
            return false;
        }
    }
}
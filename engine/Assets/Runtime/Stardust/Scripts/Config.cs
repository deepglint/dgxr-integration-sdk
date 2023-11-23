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
            RightHandDrawCircle = 1,  //左手画圈
            LeftHandDrawCircle = 2,   //右手画圈
            Kick = 10,   //踢腿
            CombineHandsStraight = 17,   //双手伸直合并
            ThrowBoulder = 18,   //举手投掷巨物
            SlowRun = 19,   //慢跑
            FastRun = 20,   //快跑
            Butterfly = 21,   //蝶泳
            Freestyle = 22,   //自由泳
            KeepRaisingHand = 23,   //持续举手
            Applaud = 24,   //拍掌
            Jump = 25,   //起跳
            DeepSquat = 26,   //下蹲
            RaiseOnHand = 10000, //举单手
            RaiseBothHand = 10001, //举双手
            ArmFlat = 10002, //手臂平展
            ArmFlatIsL = 10003, //手臂平展为 L
            ArmVerticalIsL = 10004, //手臂垂直为 L

            SlideLeft = 2001,// 左滑
            SlideRight = 2002,// 右滑
            SlideUp = 2003,// 上滑
            SlideDown = 2004,// 下滑
            HandsAway = 2005,// 双手远离
            HandsClose = 2006,// 双手靠近
            Waving = 2007,// 挥手
            ArmToForward = 2008,// 手臂向前
            ArmToBack = 2009,// 手臂向后
            ArmToLeft = 2010,// 手臂向左
            ArmToRight = 2011,// 手臂向右
            BendBothElbows = 2012,// 弯双肘
            HandsCross = 2013,// 双手交叉

            PoseA = 3001, // 姿势 A
            PoseB = 3002, // 姿势 B
            PoseC = 3003, // 姿势 C
            PoseD = 3004, // 姿势 D
            LeanToLeft = 3005, // 左倾斜
            LeanToRight = 3006, // 右倾斜
            Stand = 3007, // 站立

            SmallSquat = 4001,// 浅蹲
        }

        public enum Key : int
        {
            DPadUp = 0,
            DPadDown = 1,
            DPadLeft = 2,
            DPadRight = 3,
            Start = 4,
            Select = 5,
            StickLeftPress = 6,
            StickRightPress = 7,
            LeftBumper = 8,
            RightBumper = 9,
            ButtonGuide = 10,

            ButtonSouth     = 12,
	        ButtonEast      = 13,
	        ButtonWest      = 14,
	        ButtonNorth     = 15,

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
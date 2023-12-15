using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Moat;

namespace BodySource
{
    public class Config : MonoBehaviour
    {
        public enum ActionType : int
        {
            Default = 0, // 无动作
            RightHandDrawCircle = 1, // 左手画圈
            LeftHandDrawCircle = 2, // 右手画圈
            HandBevelCut = 3, // 手斜切
            HandParry = 4, // 手挡开
            HandStraightCut = 5, // 手直切
            HandTransversal = 6, // 手横切
            StraightPunch = 7, // 直拳
            ReadyStraightPunch = 8, // 蓄力直拳
            Uppercut = 9, // 上勾拳
            Kick = 10, // 踢腿
            ThrowOneHandInFists = 11, // 单手握拳投掷
            ReadyThrowOneHandInFists = 12, // 蓄力单手握拳投掷
            ReadyThrowBothHandInFists = 13, // 蓄力双手握拳投掷
            ReadyHandObliqueCut = 14, // 蓄力手斜切
            WavingOneHand = 15, // 单手挥舞
            ReadyWavingOneHand = 16, // 蓄力单手挥舞
            CombineHandsStraight = 17, // 双手伸直合并
            ThrowBoulder = 18, // 举手投掷巨物
            SlowRun = 19, // 慢跑
            FastRun = 20, // 快跑
            Butterfly = 21, // 蝶泳
            Freestyle = 22, // 自由泳
            KeepRaisingHand = 23, // 持续举手
            Applaud = 24, // 拍掌
            Jump = 25, // 起跳
            DeepSquat = 26, // 下蹲
            RaiseOnHand = 10000, // 举单手
            RaiseBothHand = 10001, // 举双手
            ArmFlat = 10002, // 手臂平展
            ArmFlatIsL = 10003, // 手臂平展为L
            ArmVerticalIsL = 10004, // 手臂垂直为L
            SlideLeft = 2001, // 左滑
            SlideRight = 2002, // 右滑
            SlideUp = 2003, // 上滑
            SlideDown = 2004, // 下滑
            HandsAway = 2005, // 双手远离
            HandsClose = 2006, // 双手靠近
            Waving = 2007, // 挥手
            ArmToForward = 2008, // 手臂向前
            ArmToBack = 2009, // 手臂向后
            ArmToLeft = 2010, // 手臂向左
            ArmToRight = 2011, // 手臂向右
            BendBothElbows = 2012, // 弯曲双肘
            HandsCross = 2013, // 双手交叉
            PoseA = 3001, // 姿态A
            PoseB = 3002, // 姿态B
            PoseC = 3003, // 姿态C
            PoseD = 3004, // 姿态D
            LeanToLeft = 3005, // 身体左倾
            LeanToRight = 3006, // 身体右倾
            Stand = 3007, // 站立
            SmallSquat = 4001 // 跳一跳下蹲 
        }

        public enum Key : int
        {
            DPadUp = 0,
            DPadDown = 1,
            DPadLeft = 2,
            DPadRight = 3,
            Start = 4,
            Select = 5,
            LeftStickPress = 6,
            RightStickPress = 7,
            LeftShoulder = 8,
            RightShoulder = 9,
            ButtonGuide = 10,
            ButtonSouth = 12,
            ButtonEast = 13,
            ButtonWest = 14,
            ButtonNorth = 15,

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

        [HideInInspector] public bool isSent = false;
        private bool isEnterApp = false;

        // Update is called once per frame
        void Update()
        {
            if (!isSent && actions != null && actions.Count > 0)
            {
                isSent = SentConfig(actions);
            }

            if (Application.isFocused && !isEnterApp && isSent)
            {
                isEnterApp = true;
                isSent = false;
            }
            else if (!Application.isFocused && isEnterApp)
            {
                isEnterApp = false;
            }
        }

        public bool SentConfig(List<Action> conf)
        {
            GameObject source = GameObject.Find("Source");
            Source sourceConnect = source.GetComponent<Source>();
            if (!sourceConnect.allowConnect)
            {
                return true;
            }

            if (sourceConnect != null && sourceConnect.webSocket != null)
            {
                string jsonString = JsonConvert.SerializeObject(conf);
                MDebug.LogFlow("send ActionConfig：" + jsonString);
                sourceConnect.webSocket.Send(jsonString);
                return true;
            }

            return false;
        }
    }
}
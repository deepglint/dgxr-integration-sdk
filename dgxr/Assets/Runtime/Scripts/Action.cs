namespace DGXR
{
    public class Action
    {
        public enum ActionType : int
        {
            RightHandDrawCircle   = 1,     //左手画圈
            LeftHandDrawCircle    = 2 ,    //右手画圈
            Kick                  = 10 ,   //踢腿
            CombineHandsStraight  = 17,    //双手伸直合并
            ThrowBoulder          = 18 ,   //举手投掷巨物
            SlowRun               = 19 ,   //慢跑
            FastRun               = 20 ,   //快跑
            Butterfly             = 21 ,   //蝶泳
            Freestyle             = 22 ,   //自由泳
            KeepRaisingHand       = 23 ,   //持续举手
            Applaud               = 24 ,   //拍掌
            Jump                  = 25 ,   //起跳
            DeepSquat             = 26 ,   //下蹲
            RaiseOnHand           = 10000 ,//举单手
            RaiseBothHand         = 10001 ,//举双手
            ArmFlat               = 10002, //手臂平展
            ArmFlatIsL            = 10003, //手臂平展为 L
            ArmVerticalIsL        = 10004, //手臂垂直为 L
        }
    }
}
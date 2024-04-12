using Newtonsoft.Json;

namespace DGXR
{
    public class MetaGearInfo
    {
        public enum Event
        {
            Unknown = 0,
            Press = 1,  //按下按键
            Move = 2, //移动
            Release = 3, //释放按键
            Click = 4, //快速点击按键
        }

        public enum Button
        {
            Unknown = 0,
            A = 1,
            B = 2,
        }

        public enum MoveAction
        {
            MoveUp = 0, //向上移动
            MoveDown = 1, //向下移动
            MoveLeft = 5, //向左移动
            MoveRight = 6, //向右移动
        }

        [System.Serializable]
        public struct Device
        {
            public string id;
            public string model;
        }

        [System.Serializable]
        public struct Params
        {
            [JsonProperty("button")]
            public Button button;
            [JsonProperty("action")]
            public MoveAction action;
            [JsonProperty("confidence")]
            public float confidence;
        }

        [System.Serializable]
        public struct MetaGearData
        {
            [JsonProperty("device")]
            public Device Device { get; set; }
            [JsonProperty("event")]
            public Event Events{ get; set; }
            [JsonProperty("params")]
            public Params Params{ get; set; }
        }
    }
    
    public class MetaGearAdapter
    {
        public void DealMsg(std_msgs.msg.String msg)
        {
            MetaGearInfo.MetaGearData info = JsonConvert.DeserializeObject<MetaGearInfo.MetaGearData>(msg.Data);
            Global.TriggerMetaGearDataReceived(info);
        }
    }
}
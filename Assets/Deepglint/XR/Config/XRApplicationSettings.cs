using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Deepglint.XR.Config
{
    [System.Serializable]
    public struct PlayerSetting
    {
        [JsonProperty("minPlayerCount")] public int minPlayerCount;
        [JsonProperty("maxPlayerCount")] public int maxPlayerCount;
    }

    public class ExitButtonConfig
    {
        // 是否显示
        public bool Enable;
        // 按钮文案
        public string ButtonText;
        // 触发中提示文案
        public string ExitingInfo;
        // 达到触发时间的回调
        public Action OnExit;
    }

    [System.Serializable]
    public class Toolkit
    {
        public ExitButtonConfig ExitButtonConfig = new ExitButtonConfig()
        {
            Enable = true,
            ButtonText = "退出",
            ExitingInfo = "退出应用",
            OnExit = null,
        };
        public bool enableLoseFocusTip = true;
    }

    public class XRApplicationSettings : ScriptableObject
    {
        [HideInInspector] [JsonProperty("id")] public string id;

        [HideInInspector] [JsonProperty("name")]
        public string name;

        [HideInInspector] [JsonProperty("version")]
        public string version;

        [JsonProperty("type")] public string type = "灵境";
        [JsonProperty("player")] public PlayerSetting playerSetting;
        [JsonProperty("description")] public string description = "格灵深瞳灵境应用程序";
        [JsonProperty("toolkit")] public Toolkit toolkit;
    }
}
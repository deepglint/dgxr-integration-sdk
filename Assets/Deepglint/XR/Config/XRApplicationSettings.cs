using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deepglint.XR.Config
{
    [System.Serializable]
    public struct PlayerSetting
    {
        [JsonProperty("minPlayerCount")] public int minPlayerCount;
        [JsonProperty("maxPlayerCount")] public int maxPlayerCount;
    }

    [System.Serializable]
    public class Toolkit
    {
        public bool enableExitButton = true;
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
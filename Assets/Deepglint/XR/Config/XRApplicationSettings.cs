using Newtonsoft.Json;
using UnityEngine;

namespace Deepglint.XR.Config
{
    [System.Serializable]
    public struct PlayerSetting
    {
        [JsonProperty("minPlayerCount")]
        public int minPlayerCount;
        [JsonProperty("maxPlayerCount")]
        public int maxPlayerCount;
    }
    
    [CreateAssetMenu(fileName = "XRApplicationSettings", menuName = "Settings/XRApplication Settings")]
    public class XRApplicationSettings : ScriptableObject
    {
        [JsonProperty("id")]
        public string id;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("version")]
        public string version;
        [JsonProperty("type")]
        public string type = "灵境";
        [JsonProperty("player")]
        public PlayerSetting playerSetting;
        [JsonProperty("description")]
        public string description = "格灵深瞳灵境应用程序";
    }
}
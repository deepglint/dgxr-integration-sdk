using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DeepGlint.XR
{
    public class Config
    {
        public class ConfigData
        {
            [System.Serializable]
            public struct ConfigInfo
            {
                [JsonProperty("log")]
                public LogInfo Log { get; set; }
                [JsonProperty("space")]
                public SpaceInfo Space{ get; set; }
                [JsonProperty("record")]
                public RecordInfo Record { get; set; }
                
                
            }

            [System.Serializable]
            public struct RecordInfo
            {
                [JsonProperty("saveDay")] 
                public int SaveDay { get; set; }
                [JsonProperty("savePath")]
                public string SavePath{ get; set; }
            }

            [System.Serializable]
            public struct LogInfo 
            {
                [JsonProperty("level")]
                public string Level { get; set; }
                [JsonProperty("singFileMaxSize")]
                public int SingFileMaxSize{ get; set; }
                [JsonProperty("saveDay")]
                public int SaveDay{ get; set; }
                [JsonProperty("savePath")]
                public string SavePath{ get; set; }
            }
            
            [System.Serializable]
            public struct SpaceInfo{
                [JsonProperty("engineHost")]
                public string EngineHost { get; set; }
                [JsonProperty("wsPort")]
                public string WsPort { get; set; }
                [JsonProperty("xDirection")]
                public string XDirection { get; set; }
                [JsonProperty("zDirection")]
                public string ZDirection{ get; set; }
                [JsonProperty("screens")]
                public List<ScreenInfo> Screens { get; set; }
            }
        }

        [System.Serializable]
        public struct ScreenInfo
        {
            [JsonProperty("display")]
            public int Display { get; set; }
            [JsonProperty("render")]
            public RenderInfo[] Render { get; set; } 
            [JsonProperty("position")]
            public CoordinateInfo Position{ get; set; }
            [JsonProperty("rotation")]
            public CoordinateInfo Rotation{ get; set; }
            [JsonProperty("size")]
            public CoordinateInfo Size{ get; set; }
        }
        [System.Serializable]
        public struct RenderInfo
        {
            [JsonProperty("display")]
            public int Display{ get; set; }
            [JsonProperty("rect")]
            public float[] Rect{ get; set; }
        }

        [System.Serializable]
        public struct CoordinateInfo
        {
            [JsonProperty("x")]
            public float x{ get; set; }
            [JsonProperty("y")]
            public float y{ get; set; }
            [JsonProperty("z")]
            public float z{ get; set; }
        }


        private static string ReadData()
        {
            string filePath = "D:\\meta\\env\\config.json";
            var path = Path.GetDirectoryName(filePath);
            if(Global.SystemName.Contains("Mac"))
            {
                using StreamReader srt =File.OpenText(Path.Combine(Application.streamingAssetsPath, "config.json"));
                var data = srt.ReadToEnd();
                srt.Close();
                return data; 
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            

            if (!File.Exists(filePath))
            {
                // copy 文件到env
                string streamingAssetsPath = Application.streamingAssetsPath;
                string sourceFilePath = Path.Combine(streamingAssetsPath, "config.json");
                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, filePath);
                }
                else
                {
                    Debug.LogError("config.json does not exist in StreamingAssets folder.");
                    Application.Quit();
                }
            }
            
            using StreamReader sr =File.OpenText(filePath);
            var readData = sr.ReadToEnd();
            sr.Close();
            return readData;
        }
        public ConfigData.ConfigInfo InitConfig()
        { 
            var info = JsonConvert.DeserializeObject<ConfigData.ConfigInfo>(ReadData());
            return info;
        }
    }
}
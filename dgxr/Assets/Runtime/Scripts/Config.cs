using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Unity.XR.DGXR
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
                [JsonProperty("source")]
                public  SourceInfo Source{ get; set; }
                [JsonProperty("record")]
                public RecordInfo Record { get; set; }
            }

            [System.Serializable]
            public struct RecordInfo
            {
                [JsonProperty("saveDay")] 
                public int SaveDay { get; set; }
                [JsonProperty("cacheFrameSize")]
                public int CacheFrameSize { get; set; }
                [JsonProperty("savePath")]
                public string SavePath{ get; set; }
            }
            [System.Serializable]
            public struct SourceInfo
            {
                [JsonProperty("maxCacheFrame")] 
                public int MaxCacheFrame { get; set; }
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
                [JsonProperty("xDirection")]
                public string XDirection { get; set; }
                [JsonProperty("zDirection")]
                public string ZDirection{ get; set; }
                
            }
        }


        private static string ReadData()
        {
            string filePath = "C:\\meta\\env\\config.json";
            var path = Path.GetDirectoryName(filePath);
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
                    GameLogger.LogError("config.json does not exist in StreamingAssets folder.");
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
using System.Collections.Generic;
using System.IO;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.DebugTool;
using Newtonsoft.Json;
using UnityEngine;

namespace Deepglint.XR.Config
{
    /// <summary>
    /// 配置类，根据空间和操作系统读取配置文件
    /// </summary>
    public class Config
    {
        private const string ConfigName = "env.json";
        private const string FilePath = @"D:\meta\env\env.json";

        public class ConfigData
        {
            [System.Serializable]
            public struct ConfigInfo
            {
                [JsonProperty("debug")] public bool Debug { get; set; }

                [JsonProperty("log")] public LogInfo Log { get; set; }
                [JsonProperty("space")] public SpaceInfo Space { get; set; }
                [JsonProperty("record")] public RecordInfo Record { get; set; }
            }

            [System.Serializable]
            public struct RecordInfo
            {
                [JsonProperty("saveDay")] public int SaveDay { get; set; }
                [JsonProperty("savePath")] public string SavePath { get; set; }
            }

            [System.Serializable]
            public struct LogInfo
            {
                [JsonProperty("enable")] public bool Enable { get; set; }
                [JsonProperty("level")] public string Level { get; set; }
                [JsonProperty("singFileMaxSize")] public int SingFileMaxSize { get; set; }
                [JsonProperty("saveDay")] public int SaveDay { get; set; }
                [JsonProperty("savePath")] public string SavePath { get; set; }
            }

            [System.Serializable]
            public struct SpaceInfo
            {
                [JsonProperty("name")] public string Name { get; internal set; }

                [JsonProperty("id")] public string ID { get; internal set; }

                [JsonProperty("screenMode")] public ScreenStyle ScreenMode { get; internal set; }

                [JsonProperty("engineHost")] public string EngineHost { get; set; }
                [JsonProperty("wsPort")] public string WsPort { get; set; }
                [JsonProperty("xDirection")] public string XDirection { get; set; }
                [JsonProperty("zDirection")] public string ZDirection { get; set; }
                [JsonProperty("length")] public float Length { get; set; }
                [JsonProperty("width")] public float Width { get; set; }
                [JsonProperty("height")] public float Height { get; set; }
                [JsonProperty("roi")] public float[] Roi { get; set; }
                [JsonProperty("screens")] public List<ScreenConfig> Screens { get; set; }
            }
        }

        [System.Serializable]
        public class ScreenConfig
        {
            [JsonProperty("display")] public TargetScreen TargetScreen { get; set; }

            [JsonProperty("render")] public RenderInfo[] Render { get; set; }

            [JsonProperty("position")] public CoordinateInfo Position { get; set; }
            [JsonProperty("rotation")] public CoordinateInfo Rotation { get; set; }
            [JsonProperty("size")] public CoordinateInfo Scale { internal get; set; }
        }

        [System.Serializable]
        public struct RenderInfo
        {
            [JsonProperty("display")] public int Display { get; set; }
            [JsonProperty("tarDisplay")] public int[] TarDisplay { get; set; }
            [JsonProperty("rect")] public float[] Rect { get; set; }
        }

        [System.Serializable]
        public struct CoordinateInfo
        {
            [JsonProperty("x")] public float x { get; set; }
            [JsonProperty("y")] public float y { get; set; }
            [JsonProperty("z")] public float z { get; set; }
        }

        /// <summary>
        /// 读取空间配置文件，根据操作系统不同、空间不同、环境不同读取相应的文件
        /// </summary>
        private static string ReadData()
        {
            var path = Path.GetDirectoryName(FilePath);
            if (Application.isEditor || DGXR.SystemName.Contains("Mac"))
            {
                string packagePath = Path.GetFullPath(Path.Combine("Packages", DGXR.PackageName));
                string envJsonPath = Path.Combine(packagePath, "StreamingAssets");
                string streamingAssetsPath = Application.streamingAssetsPath;
                if (!Directory.Exists(streamingAssetsPath))
                {
                    Directory.CreateDirectory(streamingAssetsPath);
                }

                if (Directory.Exists(envJsonPath))
                {
                    string[] files = Directory.GetFiles(envJsonPath);
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(streamingAssetsPath, fileName);
                        if (!File.Exists(destFile))
                        {
                            File.Copy(file, destFile, true);
                        }
                    }
                }

                using StreamReader srt = File.OpenText(Path.Combine(Application.streamingAssetsPath, ConfigName));
                var data = srt.ReadToEnd();
                srt.Close();
                return data;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(FilePath))
            {
                string streamingAssetsPath = Application.streamingAssetsPath;
                string sourceFilePath = Path.Combine(streamingAssetsPath, ConfigName);
                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, FilePath);
                }
                else
                {
                    Debug.LogError("env.json does not exist in StreamingAssets folder.");
                    Application.Quit();
                }
            }

            using StreamReader sr = File.OpenText(FilePath);
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
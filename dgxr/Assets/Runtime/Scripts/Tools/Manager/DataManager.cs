using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Runtime.Scripts.Model;
using UnityEngine;

namespace Runtime.Scripts.Tools.Manager
{
    public static class DataManager
    {
        private const string ConfigSubPath = "json";
        public static Config Config { get; private set; }
        public static T ReadJsonFile<T>(string filePath)
        {
            var readData = File.ReadAllText(filePath);
            // 支持数组嵌套的情况
            return JsonConvert.DeserializeObject<T>(readData);
        }

        public static void Init()
        {
            Config = LoadData<Config>();
        }
        
        private static T LoadData<T>()
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<DataFileAttribute>();
            string name = attribute != null ? attribute.Name : $"{type.Name.ToLower()}.json";
            string path = Path.Combine(Application.streamingAssetsPath, ConfigSubPath, name);
            return DataManager.ReadJsonFile<T>(path);
        }

    }
}
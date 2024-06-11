using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public static class DataUtil
    {
        private const string ConfigSubPath = "json";
        
        public static T LoadData<T>()
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<DataFileAttribute>();
            var name = attribute != null ? attribute.Name : $"{type.Name.ToLower()}.json";
            var path = Path.Combine(Application.streamingAssetsPath, ConfigSubPath, name);
            return ReadJsonFile<T>(path);
        }
        
        public static T LoadData<T>(string filePath)
        {
            return ReadJsonFile<T>(filePath + ".json");
        }
        
        public static T ReadJsonFile<T>(string filePath)
        {
            var readData = File.ReadAllText(filePath);
            // 支持数组嵌套的情况
            return JsonConvert.DeserializeObject<T>(readData);
        }
        
        public static T LoadResourcesFile<T>(string filePath)
        {
            var readData = Resources.Load<TextAsset>(filePath);
            // 支持数组嵌套的情况
            return JsonConvert.DeserializeObject<T>(readData.text);
        }
    }
}
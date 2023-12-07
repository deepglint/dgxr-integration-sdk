using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Moat
{
    public class MReadData : MSingleton<MReadData>
    {
        public static T ReadJsonFile<T>(string filePath)
        {
            string readData = File.ReadAllText(filePath);
            // 支持数组嵌套的情况
            return JsonConvert.DeserializeObject<T>(readData);
        }

        public static string[] ReadTxtFile(string filePath)
        {
            string[] readData = File.ReadAllLines(filePath);
            return readData[0].Split(',');
        }

        private void MergeData<T>(T target, T source, string fileName)
        {
            string targetJson = JsonUtility.ToJson(target);
            string sourceJson = JsonUtility.ToJson(source);

            JsonUtility.FromJsonOverwrite(targetJson, sourceJson);
        }
    } 
}
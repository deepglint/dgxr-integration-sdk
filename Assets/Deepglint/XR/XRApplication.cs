using System.IO;
using System.Text;
using Deepglint.XR.Config;
using Deepglint.XR.Inputs;
using Newtonsoft.Json;
#if UNITY_EDITOR
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Deepglint.XR
{
#if UNITY_EDITOR    
    public class XRApplication : EditorWindow, IPreprocessBuildWithReport
    {
        private XRApplicationSettings settings;
        
        [MenuItem("Window/XRApplication Settings")]
        public static void ShowWindow()
        {
            GetWindow<XRApplication>("DGXR Application Settings");
        }

        private string GetMD5Hash(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为16进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        
        private void OnEnable()
        {
            settings = AssetDatabase.LoadAssetAtPath<XRApplicationSettings>("Assets/Resources/XRApplicationSettings.asset");
            if (settings == null)
            {
                settings = CreateInstance<XRApplicationSettings>();
                settings.name = Application.productName;
                settings.id = GetMD5Hash(settings.name).Substring(0,8);
                settings.version = Application.version;
                settings.playerSetting.minPlayerCount = 1;
                settings.playerSetting.maxPlayerCount = 6;
                AssetDatabase.CreateAsset(settings, "Assets/Resources/XRApplicationSettings.asset");
                AssetDatabase.SaveAssets();
                DGXR.Settings = settings;
                DeviceManager.MaxActiveHumanDeviceCount = settings.playerSetting.maxPlayerCount;
            }
        }
        
        private void OnGUI()
        {
            settings.type = EditorGUILayout.TextField("Application Type", settings.type);
            settings.playerSetting.minPlayerCount = EditorGUILayout.IntField("Minimum Player Count", settings.playerSetting.minPlayerCount);
            settings.playerSetting.maxPlayerCount = EditorGUILayout.IntField("Maximum Player Count", settings.playerSetting.maxPlayerCount);
            settings.description = EditorGUILayout.TextField("Description", settings.description);
            if (settings.playerSetting.maxPlayerCount <= 0 || settings.playerSetting.maxPlayerCount >= 6)
            {
                settings.playerSetting.maxPlayerCount = 6;
            }
            if (settings.playerSetting.minPlayerCount <= 0)
            {
                settings.playerSetting.minPlayerCount = 1;
            } 
            if (GUILayout.Button("Save Settings"))
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                DGXR.Settings = settings;
                DeviceManager.MaxActiveHumanDeviceCount = settings.playerSetting.maxPlayerCount;
                Debug.Log("XRApplication Settings Saved");
            }
        }

        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
            string filePath = "Assets/StreamingAssets/application.json";
            Debug.Log("load applications settings2");
            settings = AssetDatabase.LoadAssetAtPath<XRApplicationSettings>("Assets/Resources/XRApplicationSettings.asset");
            if (settings == null)
            {
                settings = CreateInstance<XRApplicationSettings>();
                settings.name = Application.productName;
                settings.id = GetMD5Hash(settings.name).Substring(0,8);
                settings.version = Application.version;
                settings.playerSetting.minPlayerCount = 1;
                settings.playerSetting.maxPlayerCount = 6;
                AssetDatabase.CreateAsset(settings, "Assets/Resources/XRApplicationSettings.asset");
                AssetDatabase.SaveAssets();
                string content = JsonConvert.SerializeObject(settings);
                File.WriteAllText(filePath, content);
            }
            else
            {
                settings.name = Application.productName;
                settings.version = Application.version;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                string content = JsonConvert.SerializeObject(settings);
                File.WriteAllText(filePath, content);
            }
        }
    }
#endif    
}
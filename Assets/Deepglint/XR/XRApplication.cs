using System.IO;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Deepglint.XR
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
    
#if UNITY_EDITOR    
    public class XRApplication : EditorWindow, IPreprocessBuildWithReport
    {
        private XRApplicationSettings settings;
        
        [MenuItem("Window/XRApplication Settings")]
        public static void ShowWindow()
        {
            GetWindow<XRApplication>("DGXR Application Settings");
        }
        
        private void OnEnable()
        {
            Debug.Log("load application settings");
            settings = AssetDatabase.LoadAssetAtPath<XRApplicationSettings>("Assets/Resources/XRApplicationSettings.asset");
            if (settings == null)
            {
                settings = CreateInstance<XRApplicationSettings>();
                settings.name = Application.productName;
                settings.version = Application.version;
                settings.playerSetting.minPlayerCount = 1;
                settings.playerSetting.maxPlayerCount = 5;
                AssetDatabase.CreateAsset(settings, "Assets/Resources/XRApplicationSettings.asset");
                AssetDatabase.SaveAssets();
            }
        }
        
        private void OnGUI()
        {
            settings.type = EditorGUILayout.TextField("Application Type", settings.type);
            settings.playerSetting.minPlayerCount = EditorGUILayout.IntField("Minimum Player Count", settings.playerSetting.minPlayerCount);
            settings.playerSetting.maxPlayerCount = EditorGUILayout.IntField("Maximum Player Count", settings.playerSetting.maxPlayerCount);
            settings.description = EditorGUILayout.TextField("Description", settings.description);
            if (settings.playerSetting.maxPlayerCount <= 0 || settings.playerSetting.maxPlayerCount >= 5)
            {
                settings.playerSetting.maxPlayerCount = 5;
            }
            if (settings.playerSetting.minPlayerCount <= 0)
            {
                settings.playerSetting.minPlayerCount = 1;
            } 
            if (GUILayout.Button("Save Settings"))
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
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
                settings.version = Application.version;
                settings.playerSetting.minPlayerCount = 1;
                settings.playerSetting.maxPlayerCount = 5;
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
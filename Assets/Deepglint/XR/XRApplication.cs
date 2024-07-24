using System;
using System.IO;
using System.Text;
using Deepglint.XR.Config;
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
        private static XRApplicationSettings _settings;

        [MenuItem("DGXR/XRApplication Settings")]
        public static void ShowWindow()
        {
            GetWindow<XRApplication>("DGXR Application Settings");
        }

        private static void InitXRApplicationSettings()
        {
            _settings = CreateInstance<XRApplicationSettings>();
            _settings.name = Application.productName;
            _settings.id = Toolkit.Utils.MD5.Hash(_settings.name).Substring(0,8);
            _settings.version = Application.version;
            _settings.playerSetting.minPlayerCount = 1;
            _settings.playerSetting.maxPlayerCount = 6;
            try
            {
                AssetDatabase.CreateAsset(_settings, "Assets/Resources/XRApplicationSettings.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            DGXR.Settings = _settings;
        }
        
        [InitializeOnLoadMethod]
        public static void CreateXRApplicationSettingsAssets()
        {
            _settings = AssetDatabase.LoadAssetAtPath<XRApplicationSettings>("Assets/Resources/XRApplicationSettings.asset");
            if (_settings == null)
            {
                // 使用 EditorApplication.delayCall 延迟初始化
                EditorApplication.delayCall += () =>
                {
                    InitXRApplicationSettings();
                };
            }
            else
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                DGXR.Settings = _settings;
            }
            
        }
        
        private void OnEnable()
        {
            _settings = AssetDatabase.LoadAssetAtPath<XRApplicationSettings>("Assets/Resources/XRApplicationSettings.asset");
            if (_settings == null)
            {
                InitXRApplicationSettings();
            }
        }

        private void OnGUI()
        {
            _settings.type = EditorGUILayout.TextField("Application Type", _settings.type);
            _settings.playerSetting.minPlayerCount =
                EditorGUILayout.IntField("Minimum Player Count", _settings.playerSetting.minPlayerCount);
            _settings.playerSetting.maxPlayerCount =
                EditorGUILayout.IntField("Maximum Player Count", _settings.playerSetting.maxPlayerCount);
            _settings.description = EditorGUILayout.TextField("Description", _settings.description);
            if (_settings.playerSetting.maxPlayerCount <= 0 || _settings.playerSetting.maxPlayerCount >= 6)
            {
                _settings.playerSetting.maxPlayerCount = 6;
            }
            if (_settings.playerSetting.minPlayerCount <= 0)
            {
                _settings.playerSetting.minPlayerCount = 1;
            }
            if (_settings.playerSetting.minPlayerCount >= _settings.playerSetting.maxPlayerCount)
            {
                _settings.playerSetting.minPlayerCount = _settings.playerSetting.minPlayerCount;
            }

            _settings.toolkit.enableExitButton =
                EditorGUILayout.Toggle("EnableExitButton", _settings.toolkit.enableExitButton);
            _settings.toolkit.enableLoseFocusTip =
                EditorGUILayout.Toggle("EnableLoseFocusTip", _settings.toolkit.enableLoseFocusTip);
        }

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            string filePath = "Assets/StreamingAssets/application.json";
            _settings.name = Application.productName;
            _settings.version = Application.version;
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
            string content = JsonConvert.SerializeObject(_settings);
            File.WriteAllText(filePath, content);
        }
    }
#endif
}
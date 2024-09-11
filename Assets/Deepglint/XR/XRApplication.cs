using System;
using System.IO;
using Deepglint.XR.Config;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Deepglint.XR
{
#if UNITY_EDITOR
    public class XRApplication : EditorWindow, IPostprocessBuildWithReport
    {
        private static XRApplicationSettings _settings;
        private static object _lock = new object();

        [MenuItem("DGXR/XRApplication Settings")]
        public static void ShowWindow()
        {
            GetWindow<XRApplication>("DGXR Application Settings");
        }

        private static void InitXRApplicationSettings()
        {
            lock (_lock)
            {
                _settings = Resources.Load<XRApplicationSettings>("XRApplicationSettings");
                if (_settings == null)
                {
                    _settings = CreateInstance<XRApplicationSettings>();
                    _settings.name = Application.productName;
                    _settings.id = Toolkit.Utils.MD5.Hash(_settings.name).Substring(0,8);
                    _settings.version = Application.version;
                    _settings.playerSetting.minPlayerCount = 1;
                    _settings.playerSetting.maxPlayerCount = 6; 
                    AssetDatabase.CreateAsset(_settings, "Assets/Resources/XRApplicationSettings.asset");
                    AssetDatabase.SaveAssets();
                } 
            }
            
            try
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                DGXR.Settings = _settings;
            }
            catch (Exception e)
            {
                DGXR.Logger.LogWarning("XRApplication", e);
            }
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
            EditorGUI.BeginChangeCheck();
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

            _settings.toolkit.ExitButtonConfig.Enable =
                EditorGUILayout.Toggle("EnableExitButton", _settings.toolkit.ExitButtonConfig.Enable);
            _settings.toolkit.enableLoseFocusTip =
                EditorGUILayout.Toggle("EnableLoseFocusTip", _settings.toolkit.enableLoseFocusTip);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                DGXR.Settings = _settings;
                DGXR.Logger.Log("XRApplication Settings Saved");
            }
        }

        public int callbackOrder => 0;
        public void OnPostprocessBuild(BuildReport report)
        {
            // 获取构建输出路径
            string buildPath = Directory.GetParent(report.summary.outputPath)?.FullName ?? string.Empty;
            // 确定构建平台，并获取 _Data 文件夹路径
            string dataFolderPath = string.Empty;
            if (report.summary.platform == BuildTarget.StandaloneWindows || report.summary.platform == BuildTarget.StandaloneWindows64)
            {
                dataFolderPath = Path.Combine(buildPath, Path.GetFileNameWithoutExtension(report.summary.outputPath) + "_Data");
            }
            else if (report.summary.platform == BuildTarget.StandaloneOSX)
            {
                dataFolderPath = Path.Combine(buildPath, "Contents", "Resources", "Data");
            }
            else if (report.summary.platform == BuildTarget.StandaloneLinux64)
            {
                dataFolderPath = Path.Combine(buildPath, Path.GetFileNameWithoutExtension(report.summary.outputPath) + "_Data");
            }
            else
            {
                DGXR.Logger.LogWarning("XRApplication", "Unsupported build platform for this script.");
                return;
            }

            string filePath = Path.Combine(dataFolderPath, "StreamingAssets", "application.json");
            DGXR.Logger.Log($"write application content to {filePath}");
            _settings.name = Application.productName;
            _settings.version = Application.version;
            string content = JsonConvert.SerializeObject(_settings);
            File.WriteAllText(filePath, content);
        }
    }
#endif
}
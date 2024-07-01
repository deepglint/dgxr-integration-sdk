using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Sentry
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class SentryTool
    {
        static SentryTool()
        {
            UpdateSentryRuntimeConfiguration();
        }
        private static void UpdateSentryRuntimeConfiguration()
        {
            string packagePath = Path.GetFullPath(Path.Combine("Packages", Global.PackageName));
            string sourcePath = Path.Combine(packagePath, "Resources/Sentry/SentryRuntimeConfiguration.asset");
            string targetAssetsPath = "Resources/Sentry";
            
            string targetPath = Path.Combine(Application.dataPath, targetAssetsPath, "SentryRuntimeConfiguration.asset");  
            
            if (!Directory.Exists(Path.GetDirectoryName(targetPath)))  
            {  
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));  
            }  
            
            if (File.Exists(sourcePath) && !File.Exists(targetPath))  
            {  
                try  
                {  
                    File.Copy(sourcePath, targetPath, true); 
                    Debug.Log("File copied successfully to " + targetPath);  
                    AssetDatabase.Refresh();  
                }  
                catch (Exception ex)  
                {  
                    Debug.LogError("Failed to copy file: " + ex.Message);  
                }  
            }  
            else  
            {  
                if (File.Exists(targetPath))  
                {  
                    Debug.Log("File already exists at " + targetPath + ", skipping copy.");  
                }  
                else  
                {  
                    Debug.Log("Source file not found at " + sourcePath);  
                }  
            }  
        }
    }
#endif
}
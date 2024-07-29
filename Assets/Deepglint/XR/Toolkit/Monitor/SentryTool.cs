using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Monitor
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    internal static class SentryTool
    {
        static SentryTool()
        {
            UpdateSentryRuntimeConfiguration();
        }

        public static void UpdateSentryRuntimeConfiguration()
        {
            string packagePath = Path.GetFullPath(Path.Combine("Packages", DGXR.PackageName));
            string sourcePath = Path.Combine(packagePath, "Resources/Sentry/SentryRuntimeConfiguration.asset");
            string targetAssetsPath = "Resources/Sentry";
            string targetPath =
                Path.Combine(Application.dataPath, targetAssetsPath, "SentryRuntimeConfiguration.asset");

            if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? string.Empty);
            }

            if (File.Exists(sourcePath))
            {
                bool shouldCopy = true;

                if (File.Exists(targetPath))
                {
                    try
                    {
                        string sourceContent = File.ReadAllText(sourcePath);
                        string targetContent = File.ReadAllText(targetPath);

                        if (sourceContent == targetContent)
                        {
                            Debug.Log("File already exists at " + targetPath +
                                      " and contents are identical, skipping copy.");
                            shouldCopy = false;
                        }
                        else
                        {
                            Debug.Log(
                                "File already exists at " + targetPath + " but contents differ, copying new file.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Failed to read files for comparison: " + ex.Message);
                    }
                }

                if (shouldCopy)
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
            }
            else
            {
                Debug.Log("Source file not found at " + sourcePath);
            }
        }
    }
#endif
}
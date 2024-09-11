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
                            DGXR.Logger.Log("File already exists at " + targetPath +
                                      " and contents are identical, skipping copy.");
                            shouldCopy = false;
                        }
                        else
                        {
                            DGXR.Logger.Log(
                                "File already exists at " + targetPath + " but contents differ, copying new file.");
                        }
                    }
                    catch (Exception ex)
                    {
                        DGXR.Logger.LogException(ex);
                    }
                }

                if (shouldCopy)
                {
                    try
                    {
                        File.Copy(sourcePath, targetPath, true);
                        DGXR.Logger.Log("File copied successfully to " + targetPath);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception ex)
                    {
                        DGXR.Logger.LogException(ex);
                    }
                }
            }
            else
            {
                DGXR.Logger.Log("Source file not found at " + sourcePath);
            }
        }
    }
#endif
}
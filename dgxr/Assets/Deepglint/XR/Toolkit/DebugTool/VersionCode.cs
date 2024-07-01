using System;
using System.IO;
using Deepglint.XR.Toolkit.Utils;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Deepglint.XR.Toolkit.DebugTool
{
    public class VersionCode : MonoBehaviour
    {
        private string _sdkVersionNumber;
        private static string _sdkVersionFilePath;
        private void Start()
        {
            _sdkVersionNumber = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "sdk_versions.txt"));
        }

        private void OnGUI()
        {
            var style = UIUtils.CreateGUIStyle(40, Color.white);
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            string appVersionNumber = "APP " + Application.version;
            string sdkVersionNumber = "SDK " + _sdkVersionNumber;

            Vector2 appVersionNumberSize = style.CalcSize(new GUIContent(appVersionNumber));
            Vector2 sdkVersionNumberSize = style.CalcSize(new GUIContent(sdkVersionNumber));
            
            
            GUI.Label(new Rect(screenWidth - appVersionNumberSize.x - 10, screenHeight - appVersionNumberSize.y - 10, appVersionNumberSize.x, appVersionNumberSize.y),
                appVersionNumber, style);
            GUI.Label(new Rect(screenWidth - sdkVersionNumberSize.x - 10, screenHeight - sdkVersionNumberSize.y - 50, sdkVersionNumberSize.x, sdkVersionNumberSize.y),
                sdkVersionNumber, style);
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void WriteVersionCode()
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");  
            string manifestContent = File.ReadAllText(manifestPath);
            
            try  
            {
                JObject manifestJson = JObject.Parse(manifestContent);  
                string packageName = "com.deepglint.xr";
                if (manifestJson["dependencies"] != null && manifestJson["dependencies"][packageName] != null)  
                {
                    string packageVersion = manifestJson["dependencies"][packageName].ToString();
            
                    if (File.Exists(Path.Combine(Application.streamingAssetsPath, "sdk_versions.txt")))
                    {
                        if (File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "sdk_versions.txt")) == packageVersion)
                        {
                            return;
                        }
                    }
                    
                    File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "sdk_versions.txt"), packageVersion);  
                }  
            }
            catch (Exception e)
            {
                Debug.Log("version code error: " + e);
            }
        }
#endif
    }
}
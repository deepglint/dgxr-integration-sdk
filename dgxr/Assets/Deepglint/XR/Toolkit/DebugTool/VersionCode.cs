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

        private static string _sdkVersionFilePath;
        private string _sdkVersionNumber;
        private void Start()
        {
            _sdkVersionFilePath = Path.Combine(Application.persistentDataPath, "sdk_versions.txt");
            _sdkVersionNumber = File.ReadAllText(_sdkVersionFilePath);
        }

        private void OnGUI()
        {
            var style = UIUtils.CreateGUIStyle(40, Color.white);
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            string appVersionNumber = Application.version;
            string sdkVersionNumber = _sdkVersionNumber;

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
            
                    if (File.Exists(_sdkVersionFilePath))
                    {
                        if (File.ReadAllText(_sdkVersionFilePath) == packageVersion)
                        {
                            return;
                        }
                    }
                    File.WriteAllText(_sdkVersionFilePath, packageVersion);  
                }  
            }
            catch (Exception e)
            {
                // ignored
            }
        }
#endif
    }
}
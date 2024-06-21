using System.IO;
using System.Reflection;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.DebugTool
{
    public class VersionCode : MonoBehaviour
    {
        private readonly GUIStyle _style = new();
        
        public class JsonConfig
        {
            public string version;
        }

        void OnGUI()
        {
            _style.fontSize = 40;
            _style.normal.textColor = Color.white;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            string versionNumber = Application.version;

            Vector2 textSize = _style.CalcSize(new GUIContent(versionNumber));

            string packageJsonPath = Path.Combine(UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Assets/").resolvedPath, "package.json");
            if (File.Exists(packageJsonPath))  
            {  
                string jsonContent = File.ReadAllText(packageJsonPath);  
                var packageInfo = JsonUtility.FromJson<JsonConfig>(jsonContent);  
  
                if (packageInfo != null)  
                {
                    Debug.Log("packageInfo" + packageInfo.version);
                }  
            }
            float textWidth = textSize.x;
            float textHeight = textSize.y;

            GUI.Label(new Rect(screenWidth - textWidth - 10, screenHeight - textHeight - 10, textWidth, textHeight),
                versionNumber, _style);
        }
    }
}
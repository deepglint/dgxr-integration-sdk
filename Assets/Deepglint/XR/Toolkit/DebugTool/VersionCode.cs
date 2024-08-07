using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.DebugTool
{
    internal class VersionCode : MonoBehaviour
    {
        // CI发布时赋值
        public const string SdkVersionCode = "v2.4.8-beta1";

        private void OnGUI()
        {
            var style = UIUtils.CreateGUIStyle(40, Color.white);
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            string appVersionNumber = "APP " + Application.version;
            string sdkVersionNumber = "SDK " + SdkVersionCode;

            Vector2 appVersionNumberSize = style.CalcSize(new GUIContent(appVersionNumber));
            Vector2 sdkVersionNumberSize = style.CalcSize(new GUIContent(sdkVersionNumber));


            GUI.Label(
                new Rect(screenWidth - appVersionNumberSize.x - 10, appVersionNumberSize.y,
                    appVersionNumberSize.x, appVersionNumberSize.y),
                appVersionNumber, style);
            GUI.Label(
                new Rect(screenWidth - sdkVersionNumberSize.x - 10, sdkVersionNumberSize.y - 40,
                    sdkVersionNumberSize.x, sdkVersionNumberSize.y),
                sdkVersionNumber, style);
        }
    }
}
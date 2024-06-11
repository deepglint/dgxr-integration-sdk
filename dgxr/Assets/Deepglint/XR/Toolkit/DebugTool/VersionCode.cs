using UnityEngine;

namespace Deepglint.XR.Toolkit.DebugTool
{
    public class VersionCode : MonoBehaviour
    {
        private readonly GUIStyle _style = new();

        void OnGUI()
        {
            _style.fontSize = 40;
            _style.normal.textColor = Color.white;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            string versionNumber = Application.version;

            Vector2 textSize = _style.CalcSize(new GUIContent(versionNumber));
            float textWidth = textSize.x;
            float textHeight = textSize.y;

            GUI.Label(new Rect(screenWidth - textWidth - 10, screenHeight - textHeight - 10, textWidth, textHeight),
                versionNumber, _style);
        }
    }
}
using UnityEngine;

namespace DeepGlint.Tools.DebugTool
{
    public class FPS : MonoBehaviour
    {
        private float _lastUpdateShowTime;
        private const float UpdateTime = 0.05f;
        private int _frames;
        private float _fPS;
        private Rect _guiFps;
        private readonly GUIStyle _style = new();

        private bool _showDebug;


        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Cursor.visible = false;
            _lastUpdateShowTime = Time.realtimeSinceStartup;
            _guiFps = new Rect(0, 0, 100, 100);
            _style.fontSize = 60;
            _style.normal.textColor = Color.red;
        }

        private void Update()
        {
            _frames++;
            if (Time.realtimeSinceStartup - _lastUpdateShowTime >= UpdateTime)
            {
                _fPS = _frames / (Time.realtimeSinceStartup - _lastUpdateShowTime);
                _frames = 0;
                _lastUpdateShowTime = Time.realtimeSinceStartup;
            }
        }

        private void OnGUI()
        {
            GUI.Label(_guiFps, "FPS: " + Mathf.RoundToInt(_fPS), _style);
        }
    }
}
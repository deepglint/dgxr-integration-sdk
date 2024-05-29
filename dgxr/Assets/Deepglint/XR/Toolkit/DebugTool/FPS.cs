using System.Collections;
using UnityEngine;

namespace Deepglint.XR.Toolkit.DebugTool
{
    public class FPS : MonoBehaviour
    {
        public float frequency = 0.5f;
        private float _fPS;
        private Rect _guiFps;
        private readonly GUIStyle _style = new();
        
        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Cursor.visible = false;
            _guiFps = new Rect(0, 0, 100, 100);
            _style.fontSize = 60;
            _style.normal.textColor = Color.red;
            StartCoroutine(fps());
        }
       
        private IEnumerator fps() {
            for(;;){
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(frequency);
                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;
                _fPS = frameCount / timeSpan;
            }
        }
        
        private void OnGUI()
        {
            GUI.Label(_guiFps, "FPS: " + Mathf.RoundToInt(_fPS), _style);
        }
    }
}
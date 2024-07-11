using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.RoamStick
{
    public class RoamCharacter : MonoBehaviour
    {
        private GameObject _root;
        private GameObject _eye;
        private GameObject _body;
        private GameObject _cameraTarget;
        void Start()
        {
            _root = transform.gameObject.FindChildGameObject("Root");
            _eye = transform.gameObject.FindChildGameObject("Eye");
            _body = transform.gameObject.FindChildGameObject("Body");
            _cameraTarget = transform.gameObject.FindChildGameObject("CameraTarget");
        }

        public void SetHeight(float bodyHeight)
        {
            _root.transform.localPosition = new Vector3(0, -bodyHeight, 0);
            _eye.transform.localPosition = new Vector3(0, bodyHeight - 0.3f, -0.5f);
            _body.transform.localScale = new Vector3(1, bodyHeight, 1);
            _cameraTarget.transform.localPosition = new Vector3(0, bodyHeight + 0.6f, 0);
        }

        public Vector3 GetCameraTarget()
        {
            return _cameraTarget == null ? Vector3.zero : _cameraTarget.transform.position;
        }
    }
}

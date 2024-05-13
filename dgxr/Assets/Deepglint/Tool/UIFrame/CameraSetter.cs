using System;
using Deepglint.Tool.Utils;
using Deepglint.XR;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Deepglint.Tool.UIFrame

{
    public class CameraSetter : MonoBehaviour
    {
        private void Start()
        {
            GameObject previewCameraGroup = GameObject.Find("PreviewCameraGroup");
            if (previewCameraGroup != null)
            {
                previewCameraGroup.SetActive(false);
            }

            GameObject CameraGroup = GameObject.Find("2DCameraGroup");
            foreach (TargetDisplay display in Enum.GetValues(typeof(TargetDisplay)))
            {
                Camera _camera = UIUtils.FindChildGameObject(CameraGroup, display.ToString()).GetComponent<Camera>();
                Global.UserView.Cameras[(int)display].GetUniversalAdditionalCameraData().cameraStack.Add(_camera);
            }
        }
    }
}
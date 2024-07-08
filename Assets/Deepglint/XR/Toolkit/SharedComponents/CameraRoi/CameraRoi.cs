using System;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.SharedComponents.CameraRoi
{
    internal class CameraRoi : MonoBehaviour
    {
        private RectTransform _lineTop;
        private RectTransform _lineBottom;
        private RectTransform _lineLeft;
        private RectTransform _lineRight;

        private GameObject _bottomCanvas;
        private GameObject _cameraRoiPrefab;

        private void Start()
        {
            _bottomCanvas = GameObject.Find("ToolkitCanvas/Bottom");
            _cameraRoiPrefab = Instantiate(Resources.Load<GameObject>("CameraRoi"), _bottomCanvas.transform, false);
            _cameraRoiPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero;


            _lineTop = _cameraRoiPrefab.FindChildGameObject("LineTop").GetComponent<RectTransform>();
            _lineBottom = _cameraRoiPrefab.FindChildGameObject("LineBottom").GetComponent<RectTransform>();
            _lineLeft = _cameraRoiPrefab.FindChildGameObject("LineLeft").GetComponent<RectTransform>();
            _lineRight = _cameraRoiPrefab.FindChildGameObject("LineRight").GetComponent<RectTransform>();


            var rect = DGXR.Space.Roi;
            int width = DGXR.Space.Bottom.Resolution.width / 2;
            int height = DGXR.Space.Bottom.Resolution.height / 2;

            _lineLeft.localPosition = new Vector3(rect.x, rect.y + rect.height / 2, 0);
            _lineLeft.gameObject.FindChildGameObject("imgLine").GetComponent<RectTransform>().sizeDelta =
                new(4, rect.height);
            if (Math.Abs(Math.Abs(rect.x) - width) == 0) _lineLeft.gameObject.SetActive(false);

            _lineRight.transform.localPosition = new Vector3(rect.x + rect.width, rect.y + rect.height / 2, 0);
            _lineRight.gameObject.FindChildGameObject("imgLine").GetComponent<RectTransform>().sizeDelta =
                new(4, rect.height);
            if (Math.Abs(Math.Abs(rect.x + rect.width) - width) == 0) _lineRight.gameObject.SetActive(false);

            _lineTop.transform.localPosition = new Vector3(rect.x + rect.width / 2, rect.height + rect.y, 0);
            _lineTop.gameObject.FindChildGameObject("imgLine").GetComponent<RectTransform>().sizeDelta =
                new(4, rect.width);
            if (Math.Abs(Math.Abs(rect.height + rect.y) - height) == 0) _lineTop.gameObject.SetActive(false);

            _lineBottom.transform.localPosition = new Vector3(rect.x + rect.width / 2, rect.y, 0);
            _lineBottom.gameObject.FindChildGameObject("imgLine").GetComponent<RectTransform>().sizeDelta =
                new(4, rect.width);
            if (Math.Abs(Math.Abs(rect.y) - height) == 0) _lineBottom.gameObject.SetActive(false);
        }
    }
}
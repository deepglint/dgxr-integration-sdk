using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.RoamStick
{
    public class RoamStick : MonoBehaviour
    {
        private enum CrossBorderType
        {
            InnerCircle,
            Toroidal,
            ExCircle
        }
        
        // stack ui
        private GameObject _stickRotation;
        private GameObject _stickBtn;

        private readonly float[] _stickRange = new float[] { 60f, 460f };
        private CrossBorderType _crossBorder;
        private float _diffAngle = 0;

        [FormerlySerializedAs("RoamDirection")] public Vector2 roamDirection;

        private void Start()
        {
            _stickRotation = gameObject.FindChildGameObject("Rotation");
            _stickBtn = gameObject.FindChildGameObject("StickBtn");
            transform.localPosition = Vector3.zero;
            _stickRange[1] = transform.localScale.x;
        }

        private float CalculateRotationAngle()
        {
            Vector2 stickBtnPos = _stickBtn.GetComponent<RectTransform>().anchoredPosition; 
            float angleInRadians = Mathf.Atan2(stickBtnPos.y, stickBtnPos.x);
            float angleInDegrees = Mathf.Rad2Deg * angleInRadians;
        
            if (angleInDegrees < 0)
            {
                angleInDegrees += 360;
            }

            return angleInDegrees - 90;
        }

        public void OnStick(Vector2 stick)
        {
            if (Vector2.Distance(_stickBtn.GetComponent<RectTransform>().anchoredPosition, Vector2.zero) < 0.1)
            {
                ControlStick(Vector2.zero);
                return;
            }
            Debug.LogFormat("stick: {0}", stick);

            _diffAngle = 45f;
            if (stick == new Vector2(0, 0))
            {
                ControlStick(new Vector2(0, 1));
            }
            else if (stick == new Vector2(1, 1))
            {
                ControlStick(new Vector2(0, -1));
            }
            else if (stick == new Vector2(1, 0))
            {
                ControlStick(new Vector2(-1, 0));
            }
            else if (stick == new Vector2(0, 1))
            {
                ControlStick(new Vector2(1, 0));
            }
        }

        public void ControlStick(Vector2 roamDirection)
        {
            this.roamDirection = roamDirection;
            if (roamDirection == Vector2.zero)
            {
                _diffAngle = 0;
                SetActiveJoystickTouch(true);
                _stickRotation.gameObject.SetActive(false);
                _stickRotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                _stickBtn.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); 
            }
            else
            {
                _stickRotation.gameObject.SetActive(true);
                float angle = CalculateRotationAngle() + _diffAngle;
                _stickRotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                _stickBtn.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

        public void SetActiveJoystickTouch(bool status)
        {
            ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
            scrollRect.enabled = status;
        }

        public bool CheckCrossBorder(Vector2 position2d)
        {
            Vector2 direction = position2d - Vector2.zero;
            float distance = direction.magnitude;
            if (distance < _stickRange[0])
            {
                _crossBorder = CrossBorderType.InnerCircle;
                _stickBtn.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else if (distance > _stickRange[1])
            {
                _crossBorder = CrossBorderType.ExCircle;
                Vector2 newPosition = direction.normalized * _stickRange[1];
                _stickBtn.GetComponent<RectTransform>().anchoredPosition = newPosition;
            }
            else
            {
                _crossBorder = CrossBorderType.Toroidal;
                _stickBtn.GetComponent<RectTransform>().anchoredPosition = position2d;
            }

            _stickRotation.gameObject.SetActive(_crossBorder == CrossBorderType.Toroidal);
            return _crossBorder != CrossBorderType.InnerCircle;
        }
    }
}
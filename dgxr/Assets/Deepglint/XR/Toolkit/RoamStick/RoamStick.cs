using System.Collections.Generic;
using Deepglint.XR.Space;
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

        // speed
        [FormerlySerializedAs("Speed")] public float speed = 1;

        // roam
        [FormerlySerializedAs("human")] [FormerlySerializedAs("Human")]
        public GameObject humanBody;

        private GameObject _xrManager;
        private Vector2 _roamDirection;

        // SmoothingFilter
        private readonly float _alpha = 0.1f;
        private Vector3 _lastSmoothedPos = Vector3.zero;
        private readonly List<Vector3> _cacheList = new List<Vector3>();
        private Vector3 _currentMovePos = Vector3.zero;
        private Vector3 _currentPos = Vector3.zero;
        
        private void SetCache(Vector3 pos)
        {
            _cacheList.Add(pos);
            _currentMovePos = Vector3.Lerp(_lastSmoothedPos, pos, _alpha);
            _lastSmoothedPos = _currentMovePos;
            if (_cacheList.Count > 10) _cacheList.RemoveAt(0);
        }

        public void SetActive(bool isShow)
        {
            transform.localScale = isShow ? Vector3.one * _stickRange[1] : Vector3.zero;
        }

        private void Start()
        {
            _stickRotation = gameObject.FindChildGameObject("Rotation");
            _stickBtn = gameObject.FindChildGameObject("StickBtn");
            transform.localPosition = Vector3.zero;
            _stickRange[1] = transform.localScale.x;

            if (!DGXR.IsRoam) gameObject.SetActive(false);
            SetActive(false);
        }

        public void Move(Vector3 position3d, Vector2 position2d)
        {
            SetActiveJoystickTouch(false);
            SetCache(position3d);
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

            MoveFromStick(_crossBorder != CrossBorderType.InnerCircle
                ? new Vector2(position3d.x, position3d.z).normalized * speed
                : Vector2.zero);
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

        public void ResetMove(bool isBackStart)
        {
            if (!isBackStart || humanBody == null) return;
            MoveFromStick(Vector2.zero);
            humanBody.transform.position = new Vector3(0, 3f, 0);
        }

        public void OnStick(Vector2 stick)
        {
            if (Vector2.Distance(_stickBtn.GetComponent<RectTransform>().anchoredPosition, Vector2.zero) < 0.1)
            {
                MoveFromStick(Vector2.zero);
                return;
            }
            Debug.LogFormat("stick: {0}", stick);

            if (stick == new Vector2(0, 0))
            {
                MoveFromStick(new Vector2(0, 1) * speed);
            }
            else if (stick == new Vector2(1, 1))
            {
                MoveFromStick(new Vector2(0, -1) * speed);
            }
            else if (stick == new Vector2(1, 0))
            {
                MoveFromStick(new Vector2(-1, 0) * speed);
            }
            else if (stick == new Vector2(0, 1))
            {
                MoveFromStick(new Vector2(1, 0) * speed);
            }
        }

        public void MoveFromStick(Vector2 roamDirection)
        {
            Debug.LogFormat("roamDirection: {0}", roamDirection);
            _roamDirection = roamDirection;

            if (roamDirection == Vector2.zero)
            {
                SetActiveJoystickTouch(true);
                _stickRotation.gameObject.SetActive(false);
                _stickRotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                _stickBtn.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); 
            }
            else
            {
                _stickRotation.gameObject.SetActive(true);
                float angle = CalculateRotationAngle();
                _stickRotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                _stickBtn.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

        private void SetActiveJoystickTouch(bool status)
        {
            ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
            scrollRect.enabled = status;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _xrManager.GetComponent<SpaceManager>().isCave = !_xrManager.GetComponent<SpaceManager>().isCave;
            }
        }

        private void FixedUpdate()
        {
            if (_xrManager == null) _xrManager = GameObject.Find("XRManager");
            if (humanBody == null)
            {
                _xrManager.transform.position = _currentPos;
                if (_roamDirection == Vector2.zero) return;
                _currentPos += (new Vector3(_roamDirection.x, 0, _roamDirection.y) * Time.deltaTime);
                return;
            }

            _xrManager.transform.position = humanBody.transform.position - _currentMovePos;
            SetCave();

            if (_roamDirection == Vector2.zero) return;
            var position = humanBody.transform.position;
            position += (new Vector3(_roamDirection.x, 0, _roamDirection.y) * Time.deltaTime);
            position = new Vector3(position.x, Mathf.Max(0f, position.y), position.z);
            humanBody.transform.position = position;
            humanBody.FindChildGameObject("Capsule").transform.localScale =
                new Vector3(0.5f, _currentMovePos.y, 0.5f);
        }

        private void SetCave()
        {
            if (_xrManager.GetComponent<SpaceManager>().isCave)
            {
                _xrManager.GetComponent<SpaceManager>().lockAll = false;
                DGXR.CavePosition = _currentMovePos;
            }
            else
            {
                DGXR.CavePosition = _xrManager.transform.position + new Vector3(0, 1.6f, 0);
            }
        }
    }
}
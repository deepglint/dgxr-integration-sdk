using System.Collections.Generic;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deepglint.XR.Toolkit.RoamStick
{
    public class RoamStick: MonoBehaviour 
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
        private float _averageDistance;
        
        // speed
        private readonly float _speedWeight = 0.5f;
        
        // roam
        [FormerlySerializedAs("human")] [FormerlySerializedAs("Human")] public GameObject humanBody;
        private GameObject _xrManager;
        private Vector2 _roamDirection;
    
        // SmoothingFilter
        private readonly float _alpha = 0.1f;
        private Vector3 _lastSmoothedPos = Vector3.zero;
        private readonly List<Vector3> _cacheList = new List<Vector3>();
        private Vector3 _currentMovePos;

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
            _averageDistance = (_stickRange[1] - _stickRange[0]) / 6;
            _stickRange[1] = Mathf.Max(400f, transform.localScale.x);

            SetActive(false);
        }

        public void Move(Vector3 position3d, Vector2 position2d, float angle)
        {
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
            _stickRotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            _stickBtn.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            
            // 根据相对于原点距离计算速度
            float diffDistance = (distance - _stickRange[0]);
            int speed = Mathf.Clamp(Mathf.RoundToInt(diffDistance / _averageDistance), 1, 6);
            UpdateRoamMoveFromStick(_crossBorder == CrossBorderType.Toroidal ? new Vector2(position3d.x, position3d.z).normalized * (speed * _speedWeight) : Vector2.zero); 
        }
       
        public void UpdateRoamMoveFromStick(Vector2 roamDirection)
        {
            _roamDirection = roamDirection;
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
            if (humanBody == null) return;
            if (_xrManager == null) _xrManager = GameObject.Find("XRManager");
            _xrManager.transform.position = humanBody.transform.position - _currentMovePos;

            if (_xrManager.GetComponent<SpaceManager>().isCave)
            {
                _xrManager.GetComponent<SpaceManager>().lockAll = false;
                Global.CavePosition = _currentMovePos;  
            }
            else
            {
                Global.CavePosition = _xrManager.transform.position + new Vector3(0, 1.6f, 0);
            }

            if (_roamDirection == Vector2.zero) return;
            var position = humanBody.transform.position;
            position += (new Vector3(_roamDirection.x, 0, _roamDirection.y) * Time.deltaTime);
            position = new Vector3(position.x, Mathf.Max(0f, position.y), position.z);
            humanBody.transform.position = position;
            humanBody.FindChildGameObject("Capsule").transform.localScale =
                new Vector3(0.5f, _currentMovePos.y, 0.5f);
        }
    }
}

using System;
using Deepglint.XR;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Samples.HumanControlInputModule
{
    public class HumanInputModuleUITest : MonoBehaviour, 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerDownHandler, 
        IPointerUpHandler,
        IPointerClickHandler
    {
        private TMP_Dropdown _dropdown;
        private Slider _slider;
        private Toggle _toggle;
        
        private Renderer _renderer;
        private Color _originalColor;

        private void Start()
        {
            // 检查摄像机上是否已经有 PhysicsRaycaster 组件
            if (Global.Space.Bottom.SpaceCamera.GetComponent<PhysicsRaycaster>() == null)
            {
                // 添加 PhysicsRaycaster 组件
                Global.Space.Bottom.SpaceCamera.gameObject.AddComponent<PhysicsRaycaster>();
                Debug.Log("PhysicsRaycaster has been added to the bottom camera.");
            }
            else
            {
                Debug.Log("Bottom camera already has a PhysicsRaycaster.");
            }
            
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
            
            _dropdown = GetComponent<TMP_Dropdown>();
            if (_dropdown != null)
            {
                _dropdown.onValueChanged.AddListener((Int32 value) =>
                {
                    Debug.LogFormat("dropdown select: {0}", _dropdown.options[value].text); 
                });
            }
            
            _slider = GetComponent<Slider>();
            if (_slider != null)
            {
                _slider.minValue = 0;
                _slider.maxValue = 100;
                _slider.onValueChanged.AddListener((float value) =>
                {
                    Debug.LogFormat("slider value: {0}", value);
                });
                Debug.Log("slider ready");
            }
            
            _toggle = GetComponent<Toggle>();
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener((bool isOn) =>
                {
                    Debug.LogFormat("{0} is {1}", _toggle.name, isOn);
                }); 
            }
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot enter {1}" : "human {0} with left foot enter {1}",
                eventData.pointerId, name);
            if (_renderer != null)
            {
                _renderer.material.color = Color.yellow;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot exit {1}" : "human {0} with left foot exit {1}",
                eventData.pointerId, name);
            if (_renderer != null)
            {
                _renderer.material.color = _originalColor;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot down at {1}" : "human {0} with left foot down {1}",
                eventData.pointerId, name);
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot up from {1}" : "human {0} with left foot up from {1}",
                eventData.pointerId, name);
            if (_renderer != null)
            {
                _renderer.material.color = Color.yellow;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot click at {1}" : "human {0} with left foot click {1}",
                eventData.pointerId, name);
        }
    }
}
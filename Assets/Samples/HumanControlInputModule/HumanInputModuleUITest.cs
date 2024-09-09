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
            if (DGXR.Space.Bottom.SpaceCamera.GetComponent<PhysicsRaycaster>() == null)
            {
                // 添加 PhysicsRaycaster 组件
                DGXR.Space.Bottom.SpaceCamera.gameObject.AddComponent<PhysicsRaycaster>();
                DGXR.Logger.Log("PhysicsRaycaster has been added to the bottom camera.");
            }
            else
            {
                DGXR.Logger.Log("Bottom camera already has a PhysicsRaycaster.");
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
                    DGXR.Logger.Log($"dropdown select: {_dropdown.options[value].text}"); 
                });
            }
            
            _slider = GetComponent<Slider>();
            if (_slider != null)
            {
                _slider.minValue = 0;
                _slider.maxValue = 100;
                _slider.onValueChanged.AddListener((float value) =>
                {
                    DGXR.Logger.Log($"slider value: {value}");
                });
                DGXR.Logger.Log("slider ready");
            }
            
            _toggle = GetComponent<Toggle>();
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener((bool isOn) =>
                {
                    DGXR.Logger.Log($"{_toggle.name} is {isOn}");
                }); 
            }
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            DGXR.Logger.Log(
                eventData.pointerId > 0 ? $"human {eventData.pointerId} right foot enter {name}" : 
                    $"human {eventData.pointerId} with left foot enter {name}");
            if (_renderer != null)
            {
                _renderer.material.color = Color.yellow;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DGXR.Logger.Log(
                eventData.pointerId > 0 ? $"human {eventData.pointerId} right foot exit {name}" : 
                    $"human {eventData.pointerId} with left foot exit {name}");
            if (_renderer != null)
            {
                _renderer.material.color = _originalColor;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            DGXR.Logger.Log(
                eventData.pointerId > 0 ? $"human {eventData.pointerId} right foot down at {name}" : 
                    $"human {eventData.pointerId} with left foot down at {name}");
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DGXR.Logger.Log(
                eventData.pointerId > 0 ? $"human {eventData.pointerId} right foot up from {name}" : 
                    $"human {eventData.pointerId} with left foot up from {name}"); 
            if (_renderer != null)
            {
                _renderer.material.color = Color.yellow;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            DGXR.Logger.Log(
                eventData.pointerId > 0 ? $"human {eventData.pointerId} right foot click at {name}" : 
                    $"human {eventData.pointerId} with left foot click at {name}"); 
        }
    }
}
using Deepglint.XR.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

namespace Deepglint.XR.EventSystem.InputModules
{
    public class HumanControlInputModule : PointerInputModule
    {
        private PointerEventData _eventData;
        
        protected override void Start()
        {
            base.Start();
            _eventData = new PointerEventData(eventSystem);
        } 
        
        public override void Process()
        {
            HandleHumanControlInput();
        }
        
        private void HandleHumanControlInput()
        {
            if (DeviceManager.m_ActiveDeviceCount > 0)
            {
                foreach (var device in DeviceManager.AllActiveXRHumanDevices)
                {
                    // handle left foot
                    _eventData.Reset();
                    _eventData.pointerId = device.deviceId;
                    var result = HandleFootInput(device.HumanBody.LeftFoot.position.value);
                    if (result == null)
                    {
                        // handle right foot
                        _eventData.Reset();
                        result = HandleFootInput(device.HumanBody.RightFoot.position.value);
                    }
                    
                    HandlePointerExitAndEnter(_eventData, result);
                }
            }
        }

        private GameObject HandleFootInput(Vector3 foot)
        {
            _eventData.displayIndex = Global.Space.Bottom.UICamera.targetDisplay;
            _eventData.position = WorldToBottomUIPosition(foot);
            // 射线投射检测UI命中
            eventSystem.RaycastAll(_eventData, m_RaycastResultCache);
            var raycastResult = FindFirstRaycast(m_RaycastResultCache);
            _eventData.pointerCurrentRaycast = raycastResult;
            m_RaycastResultCache.Clear();

            return raycastResult.gameObject;
            // 处理进入和退出事件（可根据实际情况处理点击等其他事件）
            //HandlePointerExitAndEnter(_eventData, raycastResult.gameObject); 
        }

        private Vector2 WorldToBottomUIPosition(Vector3 position)
        {
            return new Vector2(position.x * 1920 / Global.Space.Bottom.Size.x + 960,
                position.z * 1920 / Global.Space.Bottom.Size.y + 960);
        }
    }
}
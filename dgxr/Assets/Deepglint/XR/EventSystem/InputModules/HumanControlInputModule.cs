using System.Collections.Generic;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

namespace Deepglint.XR.EventSystem.InputModules
{
    [AddComponentMenu("Event/Human Control Input Module")]
    public class HumanControlInputModule : PointerInputModule
    {
        [SerializeField]
        private float footTouchThreshold = 0.03f;

        private const float DoubleClickTime = 0.3f;
        protected override void OnEnable()
        {
            base.OnEnable();
            DeviceManager.OnDeviceLost += OnDeviceLost;
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            DeviceManager.OnDeviceLost -= OnDeviceLost;
        }
        
        public override void Process()
        {
            ProcessHumanControlEvent();
        }

        private void OnDeviceLost(int deviceId)
        {
            m_PointerData.Remove(-deviceId);
            m_PointerData.Remove(deviceId);
        }
        
        private void ProcessHumanControlEvent()
        {
            if (DeviceManager.m_ActiveDeviceCount > 0)
            {
                foreach (var device in DeviceManager.AllActiveXRHumanDevices)
                {
                    // PointerEventData left = GetFootPointerEventData(device);
                    // ProcessMove(left);
                    // PointerEventData right = GetFootPointerEventData(device, true);
                    // ProcessMove(right);

                    var humanData = GetHumanControlState(device);
                    var leftFootData = humanData.GetButtonState(HumanPointerEventData.InputButton.LeftFoot).EventData;
                    //ProcessFootTouch(leftFootData);
                    ProcessHumanMove(leftFootData.ButtonData);
                    //ProcessDrag(leftFootData.ButtonData);
                    
                    var rightFootData = humanData.GetButtonState(HumanPointerEventData.InputButton.RightFoot).EventData;
                    //ProcessFootTouch(rightFootData);
                    ProcessHumanMove(rightFootData.ButtonData);
                    //ProcessDrag(rightFootData.ButtonData);
                }
            }
        }

        private void ProcessFootTouch(HumanButtonEventData data)
        {
            var pointerEvent = data.ButtonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
                
                // todo DeselectIfSelectionChanged
                
                var resetDiffTime = Time.unscaledTime - pointerEvent.clickTime;
                if (resetDiffTime >= DoubleClickTime)
                {
                    pointerEvent.clickCount = 0;
                }
                
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
                var newClick = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
                if (newPressed == null)
                    newPressed = newClick;
                
                float time = Time.unscaledTime;
                
                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < DoubleClickTime)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }
                
                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;
                pointerEvent.pointerClick = newClick;
                
                pointerEvent.clickTime = time;
                
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            if (data.ReleasedThisFrame())
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // see if we mouse up on the same element that we clicked on...
                var pointerClickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerClick == pointerClickHandler && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerClick, pointerEvent, ExecuteEvents.pointerClickHandler);
                }

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;
                pointerEvent.pointerClick = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        private void ProcessHumanMove(PointerEventData pointerData)
        {
            GameObject hoverTarget = pointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerData, hoverTarget);
        }

        private HumanControlState GetHumanControlState(DGXRHumanController device)
        {
            HumanControlState humanState = new HumanControlState();
            PointerEventData leftFootData = GetFootPointerEventData(device);
            PointerEventData rightFootData = GetFootPointerEventData(device, true);
            humanState.SetButtonState(HumanPointerEventData.InputButton.LeftFoot, 
                StateForHumanButton(device, HumanPointerEventData.InputButton.LeftFoot), 
                leftFootData);
            humanState.SetButtonState(HumanPointerEventData.InputButton.RightFoot, 
                StateForHumanButton(device, HumanPointerEventData.InputButton.RightFoot), 
                rightFootData);
            return humanState;
        }

        private PointerEventData GetFootPointerEventData(DGXRHumanController device, bool right = false)
        {
            PointerEventData data;
            Vector2 currentFootPos;
            bool created;
            if (right)
            {
                created = GetPointerData(device.deviceId, out data, true);
                currentFootPos = WorldToBottomUIPosition(device.HumanBody.RightFoot.position.value);
            }
            else
            {
                created = GetPointerData(-device.deviceId, out data, true);
                currentFootPos = WorldToBottomUIPosition(device.HumanBody.LeftFoot.position.value);
            }
            data.Reset();

            if (created)
            {
                data.position = currentFootPos;
                data.delta = Vector2.zero;
            }
            else
            {
                data.delta = currentFootPos - data.position;
                data.position = currentFootPos;
            }
            
            data.displayIndex = Global.Space.Bottom.UICamera.targetDisplay;
            eventSystem.RaycastAll(data, m_RaycastResultCache);
            data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();

            return data;
        }

        private Vector2 WorldToBottomUIPosition(Vector3 position)
        {
            // Debug.Log("foot height: " + position.y);
            return new Vector2(
                position.x * Global.Space.Bottom.Resolution.width / Global.Space.Bottom.Size.x 
                + Global.Space.Bottom.Resolution.width * 0.5f, 
                position.z * Global.Space.Bottom.Resolution.height / Global.Space.Bottom.Size.y 
                + Global.Space.Bottom.Resolution.height * 0.5f);
        }

        private PointerEventData.FramePressState StateForHumanButton(DGXRHumanController device,
            HumanPointerEventData.InputButton buttonId)
        {
            var touched = GetButtonDown(device, buttonId);
            if (touched)
            {
                return PointerEventData.FramePressState.Pressed;
            }
            else
            {
                return PointerEventData.FramePressState.Released;
            }
            // todo set NotChanged
        }

        /// <summary>
        /// Check if foot is touched on the floor
        /// </summary>
        /// <param name="device"></param>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        private bool GetButtonDown(DGXRHumanController device, HumanPointerEventData.InputButton buttonId)
        {
            // todo use action to check if perform and release stage happened in one frame. 
            bool touched = false;
            switch (buttonId)
            {
                case HumanPointerEventData.InputButton.LeftFoot:
                    touched = device.HumanBody.LeftFoot.position.y.value < footTouchThreshold;
                    break;
                case HumanPointerEventData.InputButton.RightFoot:
                    touched = device.HumanBody.RightFoot.position.y.value < footTouchThreshold;
                    break;
            }
            
            //Debug.Log(touched);

            return touched;
        }

        private class HumanButtonState
        {
            public HumanButtonEventData EventData { get; set; }

            public HumanPointerEventData.InputButton Button { get; set; } = HumanPointerEventData.InputButton.LeftFoot;
        }
        
        /// <summary>
        /// Information about a human button event.
        /// </summary>
        public class HumanButtonEventData
        {
            /// <summary>
            /// The state of the button this frame.
            /// </summary>
            public PointerEventData.FramePressState ButtonState;

            /// <summary>
            /// Pointer data associated with the mouse event.
            /// </summary>
            public PointerEventData ButtonData;

            /// <summary>
            /// Was the button pressed this frame?
            /// </summary>
            public bool PressedThisFrame()
            {
                return ButtonState == PointerEventData.FramePressState.Pressed || ButtonState == PointerEventData.FramePressState.PressedAndReleased;
            }

            /// <summary>
            /// Was the button released this frame?
            /// </summary>
            public bool ReleasedThisFrame()
            {
                return ButtonState == PointerEventData.FramePressState.Released || ButtonState == PointerEventData.FramePressState.PressedAndReleased;
            }
        }

        private class HumanControlState
        {
            private List<HumanButtonState> _trackedButtons = new List<HumanButtonState>();
            
            public bool AnyPressesThisFrame()
            {
                var trackedButtonsCount = _trackedButtons.Count;
                for (int i = 0; i < trackedButtonsCount; i++)
                {
                    if (_trackedButtons[i].EventData.PressedThisFrame())
                        return true;
                }
                return false;
            }
            
            public bool AnyReleasesThisFrame()
            {
                var trackedButtonsCount = _trackedButtons.Count;
                for (int i = 0; i < trackedButtonsCount; i++)
                {
                    if (_trackedButtons[i].EventData.ReleasedThisFrame())
                        return true;
                }
                return false;
            }
            
            public HumanButtonState GetButtonState(HumanPointerEventData.InputButton button)
            {
                HumanButtonState tracked = null;
                var trackedButtonsCount = _trackedButtons.Count;
                for (int i = 0; i < trackedButtonsCount; i++)
                {
                    if (_trackedButtons[i].Button == button)
                    {
                        tracked = _trackedButtons[i];
                        break;
                    }
                }

                if (tracked == null)
                {
                    tracked = new HumanButtonState { Button = button, EventData = new HumanButtonEventData() };
                    _trackedButtons.Add(tracked);
                }
                return tracked;
            }
            
            public void SetButtonState(HumanPointerEventData.InputButton button, PointerEventData.FramePressState stateForHumanButton, PointerEventData data)
            {
                var toModify = GetButtonState(button);
                toModify.EventData.ButtonState = stateForHumanButton;
                toModify.EventData.ButtonData = data;
            }
        }
    }
}
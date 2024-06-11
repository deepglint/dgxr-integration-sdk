using System.Collections.Generic;
using Deepglint.XR.EventSystem.EventData;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace Deepglint.XR.EventSystem.InputModules
{
    /// <summary>
    ///  A InputModule designed for DGXRHumanControl input.
    /// </summary>
    [AddComponentMenu("Event/Human Control Input Module")]
    public class HumanControlFootPointerInputModule : PointerInputModule
    {
        [FormerlySerializedAs("EnableMouseEvent")] [SerializeField]
        public bool enableMouseEvent = false; 
            
        /// <summary>
        /// Determine whether the foot is on the bottom screen based on the height above the ground.
        /// </summary>
        [SerializeField]
        private float footTouchThreshold = 0.03f;

        private const float DoubleClickTime = 0.3f;
        
        private GameObject m_CurrentFocusedGameObject;
        
        private PointerEventData m_InputPointerEvent;
        
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
            if (!Application.isFocused)
            {
                return;
            }
            ProcessHumanControlEvent();
            if (enableMouseEvent)
            {
                ProcessMouseEvent();
            }
        }

        private void OnDeviceLost(InputDevice device)
        {
            m_PointerData.Remove(-device.deviceId);
            m_PointerData.Remove(device.deviceId);
        }
        
        protected void ProcessMouseEvent()
        {
            var mouseData = GetMousePointerEventData(0);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            m_CurrentFocusedGameObject = leftButtonData.buttonData.pointerCurrentRaycast.gameObject;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }
        
        protected void ProcessMousePress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                var resetDiffTime = Time.unscaledTime - pointerEvent.clickTime;
                if (resetDiffTime >= DoubleClickTime)
                {
                    pointerEvent.clickCount = 0;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
                var newClick = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = newClick;

                // Debug.Log("Pressed: " + newPressed);

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

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

                m_InputPointerEvent = pointerEvent;
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                ReleaseMouse(pointerEvent, currentOverGo);
            }
        }
        
        private void ReleaseMouse(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

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

            // redo pointer enter / exit to refresh state
            // so that if we moused over something that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }

            m_InputPointerEvent = pointerEvent;
        }
        
        
        /// <summary>
        /// Process the DGXRHumanControl event.
        /// </summary>
        private void ProcessHumanControlEvent()
        {
            if (DeviceManager.m_ActiveDeviceCount > 0)
            {
                foreach (var device in DeviceManager.AllActiveXRHumanDevices)
                {
                    var humanData = GetHumanControlState(device);
                    var leftFootData = humanData.GetButtonState(HumanPointerEventData.InputButton.LeftFoot).EventData;
                    ProcessFootTouch(leftFootData);
                    ProcessHumanMove(leftFootData.ButtonData);
                    ProcessDrag(leftFootData.ButtonData);
                    
                    var rightFootData = humanData.GetButtonState(HumanPointerEventData.InputButton.RightFoot).EventData;
                    ProcessFootTouch(rightFootData);
                    ProcessHumanMove(rightFootData.ButtonData);
                    ProcessDrag(rightFootData.ButtonData);
                }
            }
        }

        /// <summary>
        /// Process event data when foot is touching on the ground
        /// </summary>
        /// <param name="data"></param>
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
                
                DeselectIfSelectionChanged(currentOverGo, pointerEvent);
               
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
                if (currentOverGo != pointerEvent.pointerEnter)
                {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        /// <summary>
        /// Process event data when foot is moving on the ground
        /// </summary>
        /// <param name="pointerData"></param>
        private void ProcessHumanMove(PointerEventData pointerData)
        {
            GameObject hoverTarget = pointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerData, hoverTarget);
        }

        /// <summary>
        /// Retrieve data from DGXRHumanController and convert it to HumanControlState status data. 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert the foot data from 'DGXRHumanController' to 'PointerEventData'
        /// </summary>
        /// <param name="device"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private PointerEventData GetFootPointerEventData(DGXRHumanController device, bool right = false)
        {
            PointerEventData data;
            Vector2 currentFootPos;
            bool created;
            if (right)
            {
                created = GetPointerData(device.deviceId, out data, true);
                currentFootPos = WorldToBottomScreenPosition(device.HumanBody.RightFoot.position.value);
            }
            else
            {
                created = GetPointerData(-device.deviceId, out data, true);
                currentFootPos = WorldToBottomScreenPosition(device.HumanBody.LeftFoot.position.value);
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

            // data.displayIndex = Global.Space.Bottom.UICamera.targetDisplay;
            data.displayIndex = (int)Global.Space.Bottom.TargetScreen;
            eventSystem.RaycastAll(data, m_RaycastResultCache);
            data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();

            return data;
        }

        /// <summary>
        /// Convert world position to bottom screen position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2 WorldToBottomScreenPosition(Vector3 position)
        {
            var localPosition = Global.Space.gameObject.transform.InverseTransformPoint(position);
            return new Vector2(
                localPosition.x * Global.Space.Bottom.Resolution.width / Global.Space.Bottom.Size.x 
                + Global.Space.Bottom.Resolution.width * 0.5f, 
                localPosition.z * Global.Space.Bottom.Resolution.height / Global.Space.Bottom.Size.y 
                + Global.Space.Bottom.Resolution.height * 0.5f);
        }

        /// <summary>
        /// Get the state of a human button.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="buttonId"></param>
        /// <returns></returns>
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
        /// Check if foot is touching on the floor.
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

        /// <summary>
        /// State of HumanButton
        /// </summary>
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
                if (ButtonData.eligibleForClick == false)
                {
                    return ButtonState == PointerEventData.FramePressState.Pressed || ButtonState == PointerEventData.FramePressState.PressedAndReleased;
                }

                return false;
            }

            /// <summary>
            /// Was the button released this frame?
            /// </summary>
            public bool ReleasedThisFrame()
            {
                if (ButtonData.eligibleForClick == true)
                {
                    return ButtonState == PointerEventData.FramePressState.Released || ButtonState == PointerEventData.FramePressState.PressedAndReleased;
                }

                return false;
            }
        }

        /// <summary>
        /// State of DGXRHumanControl
        /// </summary>
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
        
        protected GameObject GetCurrentFocusedGameObject()
        {
            return m_CurrentFocusedGameObject;
        }
    }
}
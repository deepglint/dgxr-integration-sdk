using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Samples.HumanControlInputModule
{
    [RequireComponent(typeof(EventTrigger))]
    public class JoystickTouch : ScrollRect
    {
        /// <summary>
        /// 拖动差值
        /// </summary>
        public Vector2 stickValue;
    
        // 半径 -- 控制拖拽区域
        private float _radius;
    
        /// <summary>
        /// 移动中回调
        /// </summary>
        public System.Action<RectTransform> JoystickMoveHandle;
        
        /// <summary>
        /// 移动结束回调
        /// </summary>
        public System.Action<RectTransform> JoystickEndHandle;

        /// <summary>
        /// 摇杆是否处于使用状态
        /// </summary>
        public bool enable = false;
    
        protected override void Start()
        {
            _radius = content.sizeDelta.x * 0.5f;
            
            EventTrigger trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
            entryPointerUp.eventID = EventTriggerType.PointerUp;
            entryPointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
            trigger.triggers.Add(entryPointerUp);

            EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
            entryPointerDown.eventID = EventTriggerType.PointerDown;
            entryPointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
            trigger.triggers.Add(entryPointerDown);
        }
    
        protected override void OnEnable()
        {
            enable = false;
            stickValue = Vector2.zero;
        }
    
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            enable = true;
            //虚拟摇杆移动
            Vector3 contentPosition = content.anchoredPosition;
            if (contentPosition.magnitude > _radius)
            {
                contentPosition = contentPosition.normalized * _radius;
                SetContentAnchoredPosition(contentPosition);
            }
        }

        private void FixedUpdate()
        {
            if (enable)
            {
                JoystickMoveHandle?.Invoke(content);
                stickValue = content.anchoredPosition3D;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            enable = false;
            stickValue = Vector2.zero;
            JoystickEndHandle?.Invoke(content);
        }
    
        // 随手落下设置摇杆位置
        private void OnPointerDown(PointerEventData eventData)
        {
            // Vector2 LocalPosition;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //     GetComponent<RectTransform>(),
            //     eventData.position, 
            //     eventData.pressEventCamera, 
            //     out LocalPosition);
            // viewport.localPosition = LocalPosition;
        }

        // 抬起还原位置
        private void OnPointerUp(PointerEventData eventData)
        {
            viewport.anchoredPosition3D = Vector3.zero;
        }
    }
}
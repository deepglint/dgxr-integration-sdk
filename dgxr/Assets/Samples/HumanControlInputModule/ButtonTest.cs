using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Samples.HumanControlInputModule
{
    public class ButtonTest : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Button _button;

        void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickBtn);
            _button.onClick.AddListener(delegate()
            {
                OnClickBtn(123);
            });
            _button.onClick.AddListener(() =>
            {
                OnClickBtn(_button.name);
            });
        }

        public void OnClickBtn()
        {
            Debug.Log("click button");
        }

        public void OnClickBtn(int i)
        {
            Debug.LogFormat("click button with params: {0}", i);
        }

        public void OnClickBtn(string str)
        {
            Debug.LogFormat("click button with params: {0}", str); 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.LogFormat("human {0} enter button {1}", eventData.pointerId, _button.name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.LogFormat("human {0} exit button {1}", eventData.pointerId, _button.name);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.LogFormat("human {0} down at button {1}", eventData.pointerId, _button.name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.LogFormat("human {0} up from button {1}", eventData.pointerId, _button.name);
        }
    }
}
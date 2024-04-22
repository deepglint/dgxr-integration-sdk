using UnityEngine;
using UnityEngine.Serialization;

namespace Stardust.Scripts 
{
    public class WsError : MonoBehaviour
    {
        public static WsError Instance;

        [FormerlySerializedAs("MessageObj")] public GameObject messageObj;

        [FormerlySerializedAs("WsStatus")] [HideInInspector]public bool wsStatus;
        
        private void Awake()
        {
            Instance = this;
            Hide();
        }

        private void Update()
        {
            GameObject source = GameObject.Find("Source");
            if (source == null) return;
            Source sourceConnect = source.GetComponent<Source>();

            if (sourceConnect != null && wsStatus != sourceConnect.hasConnectSuccess)
            {
                wsStatus = sourceConnect.hasConnectSuccess;
                if (sourceConnect.hasConnectSuccess)
                {
                    Hide(); 
                }
                else
                {
                    Show();
                }
            }
        }

        public void Show()
        {
            messageObj.SetActive(true);
        }

        public void Hide()
        {
            messageObj.SetActive(false);
        }
    }
}

using Runtime.Scripts.Log;
using UnityEngine;
using Runtime.Scripts.Ros;

namespace Runtime.Scripts
{
    public class DGXRManager: MonoBehaviour
    {
        public bool isFilterZero;
        private DGXRNode _node;
        private ROS2UnityManager _ros;
        private WsPoseAdapter ws;
        public void Awake()
        {
            Global.UniqueID = SystemInfo.deviceUniqueIdentifier;
            Global.AppName = Application.productName;
            Global.SystemName = SystemInfo.operatingSystem; 
            Global.Config = new Config().InitConfig();
            GameLogger.Init(Global.Config.Log);
            if (Global.SystemName.Contains("Mac"))
            {
                ws = new WsPoseAdapter();
                ws.Start();
            }
            else
            {
                _ros = new ROS2UnityManager();
                _ros.Start();
            }
        }

        public void Start()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _node = new DGXRNode();
            }
            Global.IsFilterZero = isFilterZero;
        }

        public void Update()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _ros.FixedUpdate();
                _node.InitNode(_ros);
            }
        }
        
        public  void OnDestroy()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _ros.OnApplicationQuit();
            }
            else
            {
                ws.OnDestroy();
            }
        }
    }
}
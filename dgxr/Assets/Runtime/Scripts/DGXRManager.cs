using ROS2;
using UnityEngine;

namespace Unity.XR.DGXR
{
    public class DGXRManager: MonoBehaviour
    {
        public bool isFilterZero;
        private DGXRNode _node;
        private ROS2UnityManager _ros;
        public void Awake()
        {
            Global.UniqueID = SystemInfo.deviceUniqueIdentifier;
            Global.AppName = Application.productName;
            Global.Config = new Config().InitConfig();
            GameLogger.Init(Global.Config.Log);
            _ros = new ROS2UnityManager();
            _ros.Start();
        }

        public void Start()
        {
            _node = new DGXRNode();
            Global.IsFilterZero = isFilterZero;
        }

        public void Update()
        {
            _ros.FixedUpdate();
            _node.InitNode(_ros);
        }
        
        public  void OnDestroy()
        {
            _ros.OnApplicationQuit();
        }
    }
}
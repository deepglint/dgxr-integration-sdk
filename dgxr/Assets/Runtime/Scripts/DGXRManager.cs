using System;
using ROS2;
using Stardust.log;
using UnityEngine;

namespace DGXR
{
    public class DGXRManager: MonoBehaviour
    {
        public bool isFilterZero;
        private DGXRNode _node;
        private ROS2UnityComponent _ros;
        public void Awake()
        {
            Global.UniqueID = SystemInfo.deviceUniqueIdentifier;
            Global.AppName = Application.productName;
            Global.Config = new Config().InitConfig();
            GameLogger.Init(Global.Config.Log);
        }

        public void Start()
        {
            _node = new DGXRNode();
            _ros =  GetComponent<ROS2UnityComponent>();
            Global.IsFilterZero = isFilterZero;
        }

        public void Update()
        {
            _node.InitNode(_ros);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DGXR;
using Moat.Model;
using UnityEngine;

namespace Moat
{
    public class DGXRConfig : MonoBehaviour
    {
        public static DGXRConfig Instance;
        public bool AllowReadConfig = true;
        public float SpaceSize = 5f;
        public float HumanEye = 1.6f;
        
        public bool isCave;
        public bool isRoam;

        public bool allowSetViewCenterPoint = true;
        public float SpatialProportion;
        public float ViewHeight;
        public Vector3 SimulatedHumanEye = new Vector3(0, 1.6f, 0);
        public float FollowSpeed = 1;
        
        private GameObject envGameObject;

        private void Awake()
        {
            SpatialProportion = SpaceSize / 5;
            ViewHeight = (float)(1.6 * SpatialProportion);
            HumanEye = ViewHeight; 
            Instance = this;
            DisplayData.ReadConfig();
            if (AllowReadConfig)
            {
                isRoam = DisplayData.configDisplay.allowRoam;
                isCave = DisplayData.configDisplay.allowCave;  
            }
        }

        private void Start()
        {
            SimulatedHumanEye = new Vector3(SimulatedHumanEye.x, DGXRConfig.Instance.ViewHeight, SimulatedHumanEye.z);
            // float scale = 5 / SpaceSize;
            // if (envGameObject != null)
            // {
            //     envGameObject.transform.localScale = new Vector3(scale, scale, scale); 
            //     return;
            // }
            //
            // GameObject realMetaSpace = GameObject.Find("SmallMetaSpace");
            // if (realMetaSpace != null)
            // {
            //     realMetaSpace.transform.localScale = new Vector3(scale, scale, scale);
            // }
        }

        private void Update()
        {
            XRWorldManager.instance.SetCamera(new Vector3(
                SimulatedHumanEye.x,
                HumanEye + (HumanEye - SimulatedHumanEye.y) * FollowSpeed,
                SimulatedHumanEye.z));
        }
    }
}
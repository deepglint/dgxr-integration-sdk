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
        
        public Vector3 SimulatedHumanEye = new Vector3(0, 1.6f, 0);
        public int PlayerGroupsCount;
       
        [Header("相关配置 - 预览")]
        // 数值
        public int ReconnectMaxCount = DisplayData.ReconnectMaxCount;
        public int playerCount;
        public int debugLevel;
        public int interactionPermissionLevel;
        public float moveSpeed;
        public float SpaceFollowSpeed;
        public float SpaceUpperOrLowerOffsetProportion;
        public float roamSpeed;
        public float roamRotationSpeed;
       
        [Header("相关配置 - 设置")]
        public bool allowReadConfig = true;
        // 开关
        public bool wsConnect;
        public bool supportGamepad;
        public bool allowClose;
        public bool allowRoam;
        public bool allowCave;
        public bool allowFollowingInSinglePlayer;
        public bool forcedSubstitutionsInSinglePlayer; 
        
        private GameObject envGameObject;

        private void Awake()
        {
            Instance = this;
            DisplayData.allowReadConfig = allowReadConfig;
            DisplayData.ReadConfig();
            ReconnectMaxCount = DisplayData.configDisplay.ReconnectMaxCount;
            playerCount = DisplayData.configDisplay.playerCount;
            debugLevel = DisplayData.configDisplay.debugLevel;
            interactionPermissionLevel = DisplayData.configDisplay.interactionPermissionLevel;
            moveSpeed = DisplayData.configDisplay.moveSpeed;
            
            SetConfig();
        }

        private async void Start()
        {
            SimulatedHumanEye = new Vector3(SimulatedHumanEye.x, DisplayData.HumanEye, SimulatedHumanEye.z);

            await Task.Delay(0);
            SetCamera();
        }

        private void Update()
        {
            // XRWorldManager.instance.SetCameraPosition(SimulatedHumanEye);
            
            // CaveManager.Instance.Update();
            // CaveManager.Instance.CheckPerson();
            // SetConfig();
            SyncConfig();
            SyncData();
        }

        void SyncData()
        {
            PlayerGroupsCount = PlayerGroup.Instance.players.Count;
        }

        void SetCamera()
        {
            GameObject cameraXR = GameObject.Find("XRManager");
            GameObject camera3D = GameObject.Find("3DCameraGroup");
            
            if (DisplayData.allowCave)
            {
                if (cameraXR != null)
                {
                    Transform _currentCameraObj = cameraXR.GetComponent<Transform>();
                    XRWorldManager.instance.SetCameraPosition(new Vector3(_currentCameraObj.position.x, DisplayData.HumanEye, _currentCameraObj.position.z));
                    if (camera3D != null) camera3D?.SetActive(false);
                }
            }
            else
            {
                if (camera3D != null)
                {
                    Transform _currentCameraObj = camera3D.GetComponent<Transform>();
                    _currentCameraObj.position = new Vector3(_currentCameraObj.position.x, DisplayData.HumanEye, _currentCameraObj.position.z);
                    if (cameraXR != null) cameraXR?.SetActive(false);
                }
            }
        }

        void SetConfig()
        {
            DisplayData.allowReadConfig = allowReadConfig;
            if (DisplayData.allowReadConfig)
            {
                DisplayData.InitData(); 
            }
            else
            {
                DisplayData.wsConnect = wsConnect;
                DisplayData.IsGlobalTest = !wsConnect;
                DisplayData.supportGamepad = supportGamepad;
                DisplayData.allowClose = allowClose;
                DisplayData.allowRoam = allowRoam;
                DisplayData.allowCave = allowCave;
                DisplayData.allowFollowingInSinglePlayer = allowFollowingInSinglePlayer;
                DisplayData.forcedSubstitutionsInSinglePlayer = forcedSubstitutionsInSinglePlayer;
            } 
        }

        void SyncConfig()
        {
            wsConnect = DisplayData.wsConnect;
            supportGamepad = DisplayData.supportGamepad;
            allowClose = DisplayData.allowClose;
            allowRoam = DisplayData.allowRoam;
            allowCave = DisplayData.allowCave;
            allowFollowingInSinglePlayer = DisplayData.allowFollowingInSinglePlayer;
            forcedSubstitutionsInSinglePlayer = DisplayData.forcedSubstitutionsInSinglePlayer;
            
            SpaceFollowSpeed = DisplayData.SpaceFollowSpeed;
            SpaceUpperOrLowerOffsetProportion = DisplayData.SpaceUpperOrLowerOffsetProportion;
            roamSpeed = DisplayData.roamSpeed;
            roamRotationSpeed = DisplayData.roamRotationSpeed; 
        }
        void OnApplicationQuit()
        {
#if UNITY_EDITOR

#else
           System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
        }
    }
}
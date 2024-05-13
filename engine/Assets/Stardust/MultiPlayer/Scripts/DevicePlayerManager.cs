using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using Stardust.Model;
using Stardust.Scripts;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Stardust.MultiPlayer.Scripts
{
    public class PersonBody
    {
        public int MoveArea;
        public Vector2 MovementInput;
        public Vector2 LeftFootInput;
        public Vector2 RightFootInput;
        public float Angle;
    }
    /// <summary>
    /// 虚拟设备管理
    /// </summary>
    public class DevicePlayerManager: MonoBehaviour
    {
        public static DevicePlayerManager Instance;
        //链接服务器成功回调
        public Action OnOpenWssAction;
        //断开服务器回调
        public Action OnCloseWssAction;

        [HideInInspector]public int playerCount = 1;
        public string matchType = "Stardust.MultiPlayer.Scripts.MatchPlayer.DefaultMatch";
        [FormerlySerializedAs("PlayerPrefab")] public GameObject playerPrefab;
        //后端元数据
        public ConcurrentDictionary<string, BodyDataSource> PersonBodySource =
            new ConcurrentDictionary<string, BodyDataSource>();
        //处理后数据
        public Dictionary<string, PersonBody> PersonBodyInfo = new Dictionary<string, PersonBody>();

        // private GameObject parentGameObject;
        [HideInInspector] public GameObject source;
        private float _ratio = 1f;
        private void Awake()
        {
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                // Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
                source = GameObject.Find("Source");
                if (source != null)
                {
                    DontDestroyOnLoad(source);
                }
            }
        }

        void Start()
        {
            DisplayData.ReadConfig();
            // todo 
            _ratio = DisplayData.ConfigDisplay.Resolution.RealResolution / 1920;
            PlayerGroup.Instance.MaxCount = DisplayData.ConfigDisplay.PlayerCount;
            

            EventManager.RegisterListener(MoatGameEvent.WsConnectSuccess, OnOpenWSS);
            EventManager.RegisterListener(MoatGameEvent.WsConnectError, OnCloseWSS);
        }

        void DeleteVirtualDevices()
        {
            // // 获取所有具有相同名称的互动对象
            // GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("VirtualDevice");
            // // 遍历并销毁这些互动对象
            // foreach (GameObject obj in objectsToDestroy)
            // {
            //     // Destroy(obj);
            // }
        }

        void CreateVirtualDevices()
        {
            // MDebug.LogFlow("create virtual devices");
            InputDevice[] inputDevices = UnityEngine.InputSystem.InputSystem.devices.ToArray();
            foreach (InputDevice device in inputDevices)
            {
                // Helper.Instance.Log("device.displayName---" + device.displayName + "-" + device.name, "C");
                if (device.displayName.Contains("Xbox"))
                {
                    // PlayerPrefab.name = "Player" + device.name;
                    PlayerInput clone = PlayerInput.Instantiate(playerPrefab, pairWithDevice: device);
                    clone.name = "Player";
                    GameObject.DontDestroyOnLoad(clone);
                    // clone.transform.SetParent(parentGameObject.transform);
                }
            }

            foreach (InputDevice device in inputDevices)
            {
                if (device.displayName.Contains("Keyboard"))
                {
                    PlayerInput clone = PlayerInput.Instantiate(playerPrefab, pairWithDevice: device);
                    clone.name = "Player" + device.name;
                    GameObject.DontDestroyOnLoad(clone);
                    // clone.transform.SetParent(parentGameObject.transform);
                }
            }
        }
        
        void OnOpenWSS(EventCallBack evt)
        {
            OnOpenWssAction?.Invoke();
            CreateVirtualDevices();
        }

        void OnCloseWSS(EventCallBack evt)
        {
            OnCloseWssAction?.Invoke();
            DeleteVirtualDevices();
        }

        private void Update()
        {
            PersonBodySource = XrdgBodySource.Instance.GetData();

            Dictionary<string, PersonBody> personBodyInfoTmp = new Dictionary<string, PersonBody>();
            foreach (KeyValuePair<string, BodyDataSource> person in PersonBodySource)
            {
                string personId = (int.Parse(person.Key)).ToString();
                BodyDataSource personData = PersonBodySource[person.Key];
                JointData leftHipJointData = personData.Joints[JointType.LeftHip];
                JointData rightHipJointData = personData.Joints[JointType.RightHip];
                JointData leftFootAnkle = personData.Joints[JointType.LeftAnkle];
                JointData rightFootAnkle = personData.Joints[JointType.RightAnkle];
                JointData leftFootTiptoe = personData.Joints[JointType.LeftTiptoe];
                JointData rightFootTiptoe =  personData.Joints[JointType.RightTiptoe];
                Vector2 leftFoot = new Vector2((leftFootAnkle.X + leftFootTiptoe.X) / 2, (leftFootAnkle.Y + leftFootTiptoe.Y) / 2);
                Vector2 rightFoot = new Vector2((rightFootAnkle.X + rightFootTiptoe.X) / 2, (rightFootAnkle.Y + rightFootTiptoe.Y) / 2);
                
                Vector2 hipPos = new Vector2((leftHipJointData.X + rightHipJointData.X) / 2 + DisplayData.ConfigDisplay.HipOffsetX * _ratio,
                    (leftHipJointData.Y + rightHipJointData.Y) / 2 + DisplayData.ConfigDisplay.HipOffsetY * _ratio );
                PersonBody personInfo = new PersonBody();
                personInfo.MoveArea = RoiTools.Instance.CheckEnterArea(hipPos);
                personInfo.MovementInput = hipPos;// todo  换成root点位
                personInfo.LeftFootInput = leftFoot;
                personInfo.RightFootInput = rightFoot;
                personInfo.Angle = CalculatingAngle(personData);
                
                // ========解决骨骼数据抖的问题===========
                if (hipPos.x == 0 && hipPos.y == 0 && PersonBodyInfo.ContainsKey(personId) && PersonBodyInfo[personId] != null)
                {
                    personInfo = PersonBodyInfo[personId];
                    personBodyInfoTmp.Add(personId, personInfo);
                }
                else if (hipPos.x != 0 || hipPos.y != 0)
                {
                    personBodyInfoTmp.Add(personId, personInfo);
                }
            }

            List<string> keys = new List<string>(PersonBodyInfo.Keys);
            if (keys.Count > 0)
            {
                foreach (string personId in keys)
                {
                    if (!personBodyInfoTmp.ContainsKey(personId))
                    {
                        PersonBodyInfo[personId].MoveArea = 0;
                        //todo
                        PersonBodyInfo.Remove(personId);
                    }
                    else
                    {
                        PersonBodyInfo[personId] = personBodyInfoTmp[personId];
                    }
                }
            }

            List<string> tmpKeys = new List<string>(personBodyInfoTmp.Keys);
            if (tmpKeys.Count > 0)
            {
                foreach (string personId in tmpKeys)
                {
                    if (!PersonBodyInfo.ContainsKey(personId))
                    {
                        PersonBodyInfo.Add(personId, personBodyInfoTmp[personId]);
                    }
                }
            }

        }


        public void ClearReadyPlayer()
        {
            while (PlayerGroup.Instance.Players.Count > 0)
            {
                PlayerGroup.Instance.RemovePlayer(PlayerGroup.Instance.Players[0]);
            }
        }

        private float CalculatingAngle(BodyDataSource personData)
        {
            JointData leftShoulder = personData.Joints[JointType.LeftShoulder];
            JointData rightShoulder = personData.Joints[JointType.RightShoulder];

            var rad = Math.PI / 180;
            var lat1 = rightShoulder.X * rad;
            var lat2 = leftShoulder.X * rad;
            var lon1 = rightShoulder.Y * rad;
            var lon2 = leftShoulder.Y * rad;
            var a = Math.Sin(lon2 - lon1) * Math.Cos(lat2);
            var b = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1);

            var angleD = Math.Atan2(a, b) % (2 * Math.PI);
            var angleDegrees = angleD * (180 / Math.PI);
            return (float)Math.Round(angleDegrees, 2) - 180;
        }
        //============================= 本地测试逻辑 =================================
        private string _beControlledUserID;
        public string BeControlledUserID
        {
            get { return _beControlledUserID; }
            set
            {
                _beControlledUserID = value; 
                PlayerGroup.Instance.GetVirtualPlayerById(_beControlledUserID);
            }
        }

        public void DebugRemove(string id)
        {
            PlayerGroup.Instance.RemovePlayerById(id);
        }
        public void DebugAdd(string id)
        {
            // EventManager.Send(ActionEvent.OnRaiseOnHand, new object[1]{ id });
            VirtualPlayer player = new VirtualPlayer(id);
            PlayerGroup.Instance.AddPlayer(player);
        }

        //============================= 本地测试逻辑 end =================================

        void OnDestroy()
        {
            EventManager.RemoveListener(MoatGameEvent.WsConnectSuccess, OnOpenWSS);
            EventManager.RemoveListener(MoatGameEvent.WsConnectError, OnCloseWSS);
        }
    }
}
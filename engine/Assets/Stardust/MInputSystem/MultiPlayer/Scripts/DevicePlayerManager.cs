using System;
using System.Collections.Generic;
using UnityEngine;
using BodySource;
using System.Collections.Concurrent;
using UnityEngine.InputSystem;
using Moat.Model;

namespace Moat
{
    public class PersonBody
    {
        public int moveArea;
        public Vector2 movementInput;
        public Vector2 leftFootInput;
        public Vector2 rightFootInput;
        public float angle;
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
        
        public GameObject PlayerPrefab;
        [HideInInspector]public bool IsGlobalTest = false;
        //后端元数据
        private ConcurrentDictionary<string, BodyDataSource> personBodySource =
            new ConcurrentDictionary<string, BodyDataSource> { };
        //处理后数据
        public Dictionary<string, PersonBody> PersonBodyInfo = new Dictionary<string, PersonBody>();

        // private GameObject parentGameObject;
        public GameObject source;
        public Canvas DebugCanvas;
        public DebugPanel debugPanel;
        private float ratio = 1f;
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
            ratio = DisplayData.configDisplay.resolution.realResolution / DisplayData.configDisplay.resolution.systemWidth;
            IsGlobalTest = !DisplayData.configDisplay.wsConnect;
            PlayerGroup.Instance.maxCount = DisplayData.configDisplay.playerCount;

            if (DebugCanvas != null)
            {
                DebugCanvas.targetDisplay = DisplayData.configDisplay.targetDisplay.debug - 1;
                DebugCanvas.gameObject.SetActive(DisplayData.configDisplay.showMultiDebugCanvas);
            }

            if(IsGlobalTest || DisplayData.configDisplay.supportGamepad)
            {
                CreateVirtualDevices();
                OnOpenWssAction?.Invoke();
            }

            EventManager.RegisterListener(MoatGameEvent.WsConnectSuccess, OnOpenWSS);
            EventManager.RegisterListener(MoatGameEvent.WsConnectError, OnCloseWSS);
        }

        void DeleteVirtualDevices()
        {
            // 获取所有具有相同名称的互动对象
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("VirtualDevice");
            // 遍历并销毁这些互动对象
            foreach (GameObject obj in objectsToDestroy)
            {
                // Destroy(obj);
            }
        }

        void CreateVirtualDevices()
        {
            MDebug.LogFlow("create virtual devices");
            InputDevice[] inputDevices = InputSystem.devices.ToArray();
            foreach (InputDevice device in inputDevices)
            {
                // Helper.Instance.Log("device.displayName---" + device.displayName + "-" + device.name, "C");
                if (device.displayName.Contains("Xbox"))
                {
                    // PlayerPrefab.name = "Player" + device.name;
                    PlayerInput clone = PlayerInput.Instantiate(PlayerPrefab, pairWithDevice: device);
                    clone.name = "Player";
                    GameObject.DontDestroyOnLoad(clone);
                    // clone.transform.SetParent(parentGameObject.transform);
                }
            }

            foreach (InputDevice device in inputDevices)
            {
                if (device.displayName.Contains("Keyboard"))
                {
                    PlayerInput clone = PlayerInput.Instantiate(PlayerPrefab, pairWithDevice: device);
                    clone.name = "Player" + device.name;
                    GameObject.DontDestroyOnLoad(clone);
                    // clone.transform.SetParent(parentGameObject.transform);
                }
            }
        }
        
        void OnOpenWSS(EventCallBack evt)
        {
            if (IsGlobalTest) return;
            
            OnOpenWssAction?.Invoke();
            CreateVirtualDevices();
        }

        void OnCloseWSS(EventCallBack evt)
        {
            if (IsGlobalTest) return;
            OnCloseWssAction?.Invoke();
            //
            DeleteVirtualDevices();
        }

        private void Update()
        {
            if (!IsGlobalTest)
            {
                personBodySource = VRDGBodySource.Instance.GetData();
            }

            Dictionary<string, PersonBody> PersonBodyInfoTmp = new Dictionary<string, PersonBody>();
            string txt = "";
            foreach (KeyValuePair<string, BodyDataSource> person in personBodySource)
            {
                string personId = (int.Parse(person.Key) + 1).ToString();
                BodyDataSource personData = personBodySource[person.Key];
                JointData LeftHipJointData = personData.Joints[JointType.LeftHip];
                JointData RightHipJointData = personData.Joints[JointType.RightHip];
                JointData LeftFootAnkle = personData.Joints[JointType.LeftAnkle];
                JointData RightFootAnkle = personData.Joints[JointType.RightAnkle];
                JointData LeftFootTiptoe = personData.Joints[JointType.LeftTiptoe];
                JointData RightFootTiptoe =  personData.Joints[JointType.RightTiptoe];
                Vector2 LeftFoot = new Vector2((LeftFootAnkle.X + LeftFootTiptoe.X) / 2, (LeftFootAnkle.Y + LeftFootTiptoe.Y) / 2);
                Vector2 RightFoot = new Vector2((RightFootAnkle.X + RightFootTiptoe.X) / 2, (RightFootAnkle.Y + RightFootTiptoe.Y) / 2);
                
                Vector2 HipPos = new Vector2((LeftHipJointData.X + RightHipJointData.X) / 2 + DisplayData.configDisplay.circlePosX * ratio,
                    (LeftHipJointData.Y + RightHipJointData.Y) / 2 + DisplayData.configDisplay.circlePosY * ratio);
                PersonBody personInfo = new PersonBody();
                personInfo.moveArea = ROITools.Instance.CheckEnterArea(HipPos);
                personInfo.movementInput = HipPos;
                personInfo.leftFootInput = LeftFoot;
                personInfo.rightFootInput = RightFoot;
                personInfo.angle = CalculatingAngle(personData);
                
                // ========解决骨骼数据抖的问题===========
                if (HipPos.x == 0 && HipPos.y == 0 && PersonBodyInfo.ContainsKey(personId) && PersonBodyInfo[personId] != null)
                {
                    personInfo = PersonBodyInfo[personId];
                    PersonBodyInfoTmp.Add(personId, personInfo);
                }
                else if (HipPos.x != 0 || HipPos.y != 0)
                {
                    PersonBodyInfoTmp.Add(personId, personInfo);
                }
            }

            List<string> keys = new List<string>(PersonBodyInfo.Keys);
            if (keys.Count > 0)
            {
                foreach (string personId in keys)
                {
                    if (!PersonBodyInfoTmp.ContainsKey(personId))
                    {
                        PersonBodyInfo[personId].moveArea = 0;
                        //todo
                        PersonBodyInfo.Remove(personId);
                    }
                    else
                    {
                        PersonBodyInfo[personId] = PersonBodyInfoTmp[personId];
                    }
                }
            }

            List<string> tmpKeys = new List<string>(PersonBodyInfoTmp.Keys);
            if (tmpKeys.Count > 0)
            {
                foreach (string personId in tmpKeys)
                {
                    if (!PersonBodyInfo.ContainsKey(personId))
                    {
                        PersonBodyInfo.Add(personId, PersonBodyInfoTmp[personId]);
                    }
                }
            }

            DebugUpdate();
        }


        public void ClearReadyPlayer()
        {
            while (PlayerGroup.Instance.players.Count > 0)
            {
                PlayerGroup.Instance.RemovePlayer(PlayerGroup.Instance.players[0]);
            }

            if (IsGlobalTest && DebugCanvas != null)
            {
                debugPanel.ResetPanel();
            }
        }

        private float CalculatingAngle(BodyDataSource personData)
        {
            JointData leftShoulder = personData.Joints[JointType.LeftShoulder];
            JointData rightShoulder = personData.Joints[JointType.RightShoulder];
            float dx = rightShoulder.X - leftShoulder.X;
            float dy = rightShoulder.Y - leftShoulder.Y;
            double angleRadians = Math.Atan(dy / dx);
            double angleDegrees = angleRadians * (180 / Math.PI);
            
            return (float)Math.Round(angleDegrees,2);
        }
        //============================= 本地测试逻辑 =================================

        private float debugSpeed = 5;
        
        private string _beControlledUserID;
        public string BeControlledUserID
        {
            get { return _beControlledUserID; }
            set
            {
                _beControlledUserID = value; 
                beControlledPlayer = PlayerGroup.Instance.GetVirtualPlayerById(_beControlledUserID);
            }
        }
        private VirtualPlayer beControlledPlayer;
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

        private void DebugUpdate()
        {
            //本地调试处理
            if (IsGlobalTest)
            {
                if (beControlledPlayer == null) return;
                // 获取按键输入
                float verticalInput = 0;
                float horizontalInput = 0;
                float speedScale = 0.35f;
                if (Input.GetKey(KeyCode.DownArrow))
                    verticalInput = -speedScale;
                else if (Input.GetKey(KeyCode.UpArrow))
                    verticalInput = speedScale;
                else if (Input.GetKey(KeyCode.LeftArrow))
                    horizontalInput = -speedScale;
                else if (Input.GetKey(KeyCode.RightArrow))
                    horizontalInput = speedScale;
                if (verticalInput != 0 || horizontalInput != 0)
                {
                    Vector2 vector2 = new Vector2(horizontalInput * debugSpeed * Time.deltaTime,
                        verticalInput * debugSpeed * Time.deltaTime);
                    if (!ROITools.Instance.CheckBoundary(beControlledPlayer.movementInput + vector2)) return;
                    beControlledPlayer.movementInput += vector2;
                    beControlledPlayer.leftFootInput += vector2;
                    beControlledPlayer.rightFootInput += vector2;
                }
            }
        }


        private void Test(int count, int[] moveAreas)
        {
            if (!IsGlobalTest) return;
            personBodySource = new ConcurrentDictionary<string, BodyDataSource> { };

            for (int j = 0; j < count; j++)
            {
                BodyDataSource body = new BodyDataSource { };
                body.IsTracked = true;
                body.BodyID = j.ToString();
                body.Joints = new Dictionary<JointType, JointData> { };
                int rows = 25; // 获取行数

                for (int i = 0; i < rows; i++)
                {
                    JointData joint = new JointData(ROITools.Instance.GetTestPosX(moveAreas[j]), 0f, 0f);
                    JointType jointType = (JointType)i;
                    body.Joints.Add(jointType, joint);
                }

                personBodySource.TryAdd(body.BodyID, body);
            }
        }
        //============================= 本地测试逻辑 end =================================

        void OnDestroy()
        {
            EventManager.RemoveListener(MoatGameEvent.WsConnectSuccess, OnOpenWSS);
            EventManager.RemoveListener(MoatGameEvent.WsConnectError, OnCloseWSS);
        }
    }
}
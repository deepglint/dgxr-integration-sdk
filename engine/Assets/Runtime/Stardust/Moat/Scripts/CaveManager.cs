using System;
using System.Collections.Generic;
using UnityEngine;
using BodySource;
using System.Collections.Concurrent;
using System.Linq;

namespace Moat 
{
    public class OnePersonBody
    {
        public Vector2 movementInput;
    }
    
    public class CaveManager
    {
        private static CaveManager sInstance;
        public static CaveManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new CaveManager();
                }
                return sInstance;
            }
        }
        
        //后端元数据
        public ConcurrentDictionary<string, BodyDataSource> personBodySource =
            new ConcurrentDictionary<string, BodyDataSource> { };
        //处理后数据
        public Dictionary<string, OnePersonBody> personBodyInfo = new Dictionary<string, OnePersonBody>();

        // 当前主玩家
        private string mainPlayerId;
        
        // 是否允许活体检测
        public bool allowBodyCheck = true;
        
        // 活体检测时间
        public int checkTotalTime = 60;
        private int currentTime = 0;

        public static Action CheckNoBodyCallback;

        public void Init()
        {
        }

        // Update is called once per frame
        public void Update()
        {
            personBodySource = VRDGBodySource.Instance.GetData(); 

            Dictionary<string, OnePersonBody> personBodyInfoTmp = new Dictionary<string, OnePersonBody>();
            string txt = "";
            foreach (KeyValuePair<string, BodyDataSource> person in personBodySource)
            {
                BodyDataSource personData = personBodySource[person.Key];
                // MDebug.Log("======== personData.BodyID: " + personData.BodyID);
                string personId = (int.Parse(personData.BodyID) + 1).ToString();
                JointData LeftHipData = personData.Joints[JointType.LeftHip];
                JointData RightHipData = personData.Joints[JointType.RightHip];
                Vector2 HipPos = new Vector2((LeftHipData.X + RightHipData.X) / 2,
                    (LeftHipData.Y + RightHipData.Y) / 2);
                OnePersonBody personInfo = new OnePersonBody();
                personInfo.movementInput = HipPos;

                // 解决骨骼数据抖的问题
                if (HipPos.x == 0 && HipPos.y == 0 && personBodyInfo.Keys.Contains(personId))
                {
                    personInfo = personBodyInfo[personId];
                    personBodyInfoTmp.Add(personId, personInfo);
                }
                else if (HipPos.x != 0 || HipPos.y != 0)
                {
                    personBodyInfoTmp.Add(personId, personInfo);
                }
            }

            List<string> keys = new List<string>(personBodyInfo.Keys);
            if (keys.Count > 0)
            {
                foreach (string personId in keys)
                {
                    if (!personBodyInfoTmp.ContainsKey(personId))
                    {
                        personBodyInfo.Remove(personId);
                    }
                    else
                    {
                        personBodyInfo[personId] = personBodyInfoTmp[personId];
                    }
                }
            }

            List<string> tmpKeys = new List<string>(personBodyInfoTmp.Keys);
            if (tmpKeys.Count > 0)
            {
                foreach (string personId in tmpKeys)
                {
                    if (!personBodyInfo.ContainsKey(personId))
                    {
                        personBodyInfo.Add(personId, personBodyInfoTmp[personId]);
                    }
                }
            }

            // CheckPerson();
            // CheckNoBody();
        }

        public void SwitchPlayer(string playerId)
        {
            if (personBodyInfo.Keys.Contains(playerId))
            {
                VRDGBodySource.Instance.SetCavePersonId((int.Parse(playerId) - 1).ToString());
            }
        }

        public bool IsHasPlayer()
        {
            if (personBodyInfo.Count > 0)
            {
                return true;
            }

            return false;
        }
        
        public void CheckNoBody()
        {
            if (!allowBodyCheck) return;
            currentTime++;
            MDebug.Log("活体检测：" + currentTime);

            if (currentTime >= checkTotalTime)
            {
                CheckNoBodyCallback?.Invoke(); 
                currentTime = 0;
            }

            if (CaveManager.Instance.IsHasPlayer())
            {
                currentTime = 0;
            }
        }

        public void Test(int count, int[] moveAreas)
        {
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
                    JointData joint = new JointData(0f, 0f, 0f);
                    JointType jointType = (JointType)i;
                    body.Joints.Add(jointType, joint);
                }

                personBodySource.TryAdd(body.BodyID, body);
            }
        }

        public void CheckPerson()
        {
            foreach (KeyValuePair<string, BodyDataSource> player in personBodySource)
            {
                BodyDataSource person = personBodySource[player.Key];
            
                if ((person.Joints[JointType.RightHand].Z - person.Joints[JointType.HeadTop].Z )> 0.15) {
                    if (mainPlayerId != person.BodyID)
                    {
                        mainPlayerId = person.BodyID;
                        VRDGBodySource.Instance.SetCavePersonId(person.BodyID);
                        break;
                    }
                }
            }
        }
    }
}
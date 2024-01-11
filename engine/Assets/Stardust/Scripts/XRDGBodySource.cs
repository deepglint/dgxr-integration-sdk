using System.Collections.Concurrent;
using System.Collections.Generic;
using Moat;
using Moat.Model;
using UnityEngine;
using Newtonsoft.Json;

namespace BodySource
{
    public class XRDGBodySource
    {
        private static XRDGBodySource instance;
        public  string cavePersonId="";
        public Vector3 cavePersonHead = new Vector3(0, 0, 0);

        public ConcurrentDictionary<string, BodyDataSource> Data = new ConcurrentDictionary<string, BodyDataSource> { };
        
        // 私有构造函数，防止外部直接实例化
        public XRDGBodySource()
        {
            // 初始化GameManager
        }
        
        // 获取GameManager的实例
        public static XRDGBodySource Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XRDGBodySource();
                }
                return instance;
            }
        }
        
        public ConcurrentDictionary<string, BodyDataSource> GetData()
        {
            return Data;
        }

        public void InitBodyHead(string personId)
        {
            ConcurrentDictionary<string, BodyDataSource> personBodySource = GetData(); 
            foreach (KeyValuePair<string, BodyDataSource> person in personBodySource)
            {
                BodyDataSource personData = personBodySource[person.Key];
                Debug.Log(personId  + "当前所有玩家：personData.BodyID: " + personData.BodyID);
                string bodyId = (int.Parse(personData.BodyID) + 1).ToString();
                if (personId == bodyId)
                {
                    JointData HeadTopData = personData.Joints[JointType.HeadTop];
                    cavePersonHead = new Vector3(HeadTopData.X, HeadTopData.Y, HeadTopData.Z);
                }
            }
        }

        public void SetCavePersonId(string personId)
        {
            if (personId != cavePersonId)
            {
                MDebug.LogFlow("4. 视角跟随 - 1.1 跟随玩家 " + personId);
            }

            cavePersonId = personId;
            InitBodyHead(personId);
        }
    }
  
}

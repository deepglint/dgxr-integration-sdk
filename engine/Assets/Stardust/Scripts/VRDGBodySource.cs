using System.Collections.Concurrent;
using UnityEngine;
using Newtonsoft.Json;

namespace BodySource
{
    public class VRDGBodySource
    {
        private static VRDGBodySource instance;
        public  string cavePersonId="";

        public ConcurrentDictionary<string, BodyDataSource> Data = new ConcurrentDictionary<string, BodyDataSource> { };


        // 私有构造函数，防止外部直接实例化
        public VRDGBodySource()
        {
            // 初始化GameManager
        }
        // 获取GameManager的实例
        public static VRDGBodySource Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VRDGBodySource();
                }
                return instance;
            }
        }
        
        public ConcurrentDictionary<string, BodyDataSource> GetData()
        {
            return Data;
        }
        public void SetCavePersonId(string personId)
        {
            cavePersonId = personId;
        }
    }
  
}

using System.Collections.Concurrent;
using UnityEngine;
using Newtonsoft.Json;

namespace BodySource
{
    public class XRDGBodySource
    {
        private static XRDGBodySource instance;
        public  string cavePersonId="";

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
        public void SetCavePersonId(string personId)
        {
            cavePersonId = personId;
        }
    }
  
}

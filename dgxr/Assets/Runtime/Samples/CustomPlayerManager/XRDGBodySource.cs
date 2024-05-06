using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XrdgBodySource
{
    private static XrdgBodySource _instance;
    public  string CavePersonId="";

    public ConcurrentDictionary<string, BodyDataSource> Data = new ConcurrentDictionary<string, BodyDataSource>();
    public ConcurrentDictionary<string, InputDevice> Devices = new ConcurrentDictionary<string, InputDevice>();

    public Dictionary<string, Dictionary<string, float>> Actions =
        new Dictionary<string, Dictionary<string, float>>();
    // 私有构造函数，防止外部直接实例化

    // 获取GameManager的实例
    public static XrdgBodySource Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new XrdgBodySource();
            }
            return _instance;
        }
    }
        
    public ConcurrentDictionary<string, BodyDataSource> GetData()
    {
        return Data;
    }

    public void SetCavePersonId(string personId)
    {
        if (personId != CavePersonId)
        {
            Debug.Log("4. 视角跟随 - 1.1 跟随玩家 " + personId);
        }

        CavePersonId = personId;
    }
}
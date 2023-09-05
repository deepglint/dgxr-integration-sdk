using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BodySource
{
public class Config : MonoBehaviour
{
    public enum ActionType:int
    {
        A=0,
        B=1,
        C=2,
        D=3,
        SlowRun = 19, // 慢跑
        FastRun = 20, // 快跑
        HandUp = 23, // 举手
        CheerUp = 24,// 欢呼
        JumpUp = 25, // 起跳
        SquatDown = 26, // 下蹲
        // 添加其他可能的动作
        }

        public enum KeyType:int
    {
        DPadUp = 0,
        DPadDown = 1,
        DPadLeft = 2,
        DPadRight = 3,
        ButtonStart = 4,
        ButtonBack = 5,
        StickLeft = 6,
        StickRight = 7,
        BumperLeft = 8,
        BumperRight = 9,
        ButtonGuide = 10,
        ButtonA = 12,
        ButtonB = 13,
        ButtonX = 14,
        ButtonY = 15,

        LeftStickUp = 16,
        LeftStickDown = 17,
        LeftStickLeft = 18,
        LeftStickRight = 19,
        RightStickUp = 20,
        RightStickDown = 21,
        RightStickLeft = 22,
        RightStickRight = 23,
        LeftTrigger = 24,
        RightTrigger = 25,
    }

    [System.Serializable]
    public struct Action
    {
        public ActionType action;
        public KeyType key;
    }

    public List<Action> actions;

    private bool isSent = false;

    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
       if (!isSent) {
           isSent = SentConfig(actions);
       }
    }

    public bool SentConfig(List<Action> conf)
    {
        GameObject source = GameObject.Find("Source");
        Source sourceConnect = source.GetComponent<Source>();
        string jsonString = JsonConvert.SerializeObject(conf);
       
        if (sourceConnect != null && sourceConnect.webSocket != null)
        {
           Debug.Log(jsonString);
           if (sourceConnect.webSocket.IsOpen) {
               sourceConnect.webSocket.Send(jsonString);
               return true;
           }
        }
        return false;
    }
   }
}
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BodySource;
using UnityEngine;

namespace Moat
{
    public class BodyManager : MonoBehaviour
    {
        private ConcurrentDictionary<string, BodyDataSource> personBodySource =
            new ConcurrentDictionary<string, BodyDataSource> { };
        public GameObject PlayerPrefab;

        public Source WsSource;
        // Start is called before the first frame update
        void Start()
        {
            if (WsSource == null) return;
            WsSource.allowConnect = true;
            WsSource.init(new Options());  
        }

        // Update is called once per frame
        void Update()
        {
            personBodySource = VRDGBodySource.Instance.GetData();
            foreach (KeyValuePair<string, BodyDataSource> person in personBodySource)
            {
                string personId = (int.Parse(person.Key) + 1).ToString();
                BodyDataSource personData = personBodySource[person.Key];
                
                // 判断有没有该玩家骨骼，如果有直接过滤
                // 如果没有
                    // 判断玩家列表中是否有空闲，如果有，新创建一个
                    // 如果没有
                        // 判断玩家列表中是否有人离开
            }
        }
    }
}
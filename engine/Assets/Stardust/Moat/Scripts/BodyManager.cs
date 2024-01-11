using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodySource;
using Moat.Model;
using UnityEngine;

namespace Moat
{
    public class BodyManager : MonoBehaviour
    {
        private ConcurrentDictionary<string, BodyDataSource> personBodySource =
            new ConcurrentDictionary<string, BodyDataSource> { };

        public Camera DebugCamera;
        public GameObject PlayerPrefab;
        
        public List<string> PersonIds = new List<string> { };
        public List<string> CurrentIds = new List<string> { };
        public List<string> EmptyIds = new List<string> { };
        public List<GameObject> PlayerObjs = new List<GameObject> { };

        [Header("预览值")] public int bodyCount;

        private GameObject bodyManagerMono;

        // Start is called before the first frame update
        async void Start()
        {
            bodyManagerMono = transform.gameObject;
            bodyManagerMono.SetActive(false);
            await Task.Delay(3000);
            DisplayData.ReadConfig();
            if (DebugCamera != null) DebugCamera.targetDisplay = DisplayData.configDisplay.targetDisplay.debug - 1;
            
            GameObject source = GameObject.Find("Source");
            if (source == null) return;
            Source sourceConnect = source.GetComponent<Source>();
            if (!(sourceConnect.HasConnectSuccess && DisplayData.configDisplay.debugLevel > 3))
            {
                bodyManagerMono.SetActive(false);
                return;
            }

            bodyManagerMono.SetActive(true);
            PersonIds = new List<string> { };
            
            for (int i = 0; i < DisplayData.configDisplay.playerCount; i++)
            {
                PersonIds.Add(i.ToString());
                GameObject clone = GameObject.Instantiate(PlayerPrefab);
                clone.name = "Player" + i;
                PlayerObjs.Add(clone);
                clone.transform.SetParent(transform);
            }
        }

        // Update is called once per frame
        void Update()
        {
            personBodySource = XRDGBodySource.Instance.GetData();
            EmptyIds = new List<string>(); 
            CurrentIds = new List<string> { };

            bodyCount = personBodySource.Count;
            foreach (KeyValuePair<string, BodyDataSource> person in personBodySource)
            {
                BodyDataSource personData = personBodySource[person.Key];
                if (!CurrentIds.Contains(personData.BodyID))
                {
                    CurrentIds.Add(personData.BodyID);
                }
            }

            foreach (string personId in PersonIds)
            {
                if (!CurrentIds.Contains(personId) && !EmptyIds.Contains(personId))
                {
                    EmptyIds.Add(personId);
                }
            }

            foreach (KeyValuePair<string, BodyDataSource> person in personBodySource)
            {
                BodyDataSource personData = personBodySource[person.Key];

                GameObject currentObj = GameObject.Find("Player" + personData.BodyID);
                if (currentObj != null)
                {
                    currentObj.GetComponent<BodyRigBind>().PersonId = personData.BodyID; 
                }
                else if (!PersonIds.Contains(personData.BodyID) && PersonIds.Count < DisplayData.configDisplay.playerCount)
                {
                }
                else if (EmptyIds.Count > 0 && PersonIds.Count > 0)
                {
                    int index = PersonIds.IndexOf(EmptyIds[0]);
                    if (index > -1 && PersonIds[index] != null)
                    {
                        PersonIds[index] = personData.BodyID;
                        GameObject current = PlayerObjs[index];
                        current.name = "Player" + personData.BodyID;
                        PlayerObjs[index] = current;
                        PlayerObjs[index].SetActive(true);
                        current.GetComponent<BodyRigBind>().PersonId = personData.BodyID; 
                    }
                } 
            }
        }
    }
}
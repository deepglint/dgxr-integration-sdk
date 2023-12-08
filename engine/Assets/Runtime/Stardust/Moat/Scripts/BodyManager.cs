using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BodySource;
using Moat.Model;
using UnityEngine;

namespace Moat
{
    public class BodyManager : MonoBehaviour
    {
        private ConcurrentDictionary<string, BodyDataSource> personBodySource =
            new ConcurrentDictionary<string, BodyDataSource> { };

        public GameObject PlayerPrefab;
        public List<string> PersonIds = new List<string> { };
        public List<string> CurrentIds = new List<string> { };
        public List<string> EmptyIds = new List<string> { };
        public List<GameObject> PlayerObjs = new List<GameObject> { };

        public Source WsSource;

        // Start is called before the first frame update
        void Start()
        {
            DisplayData.ReadConfig();
            PersonIds = new List<string> { };
            if (WsSource == null) return;
            WsSource.allowConnect = true;
            WsSource.init(new Options());

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
            personBodySource = VRDGBodySource.Instance.GetData();
            EmptyIds = new List<string>(); 
            CurrentIds = new List<string> { };
            
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
                else if (EmptyIds.Count > 0)
                {
                    int index = PersonIds.IndexOf(EmptyIds[0]);
                    if (PersonIds[index] != null)
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
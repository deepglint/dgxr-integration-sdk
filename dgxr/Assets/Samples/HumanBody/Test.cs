using System.Collections;
using System.Collections.Generic;
using Deepglint.XR;
using Deepglint.XR.Source;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }
    //
    // Update is called once per frame
    void Update()
    {
        foreach (var data in Source.Data)
        {

            Debug.Log(Global.Space.Bottom.SpaceCamera.WorldToScreenPoint(data.Joints.HeadTop));
        }
       
    }
}

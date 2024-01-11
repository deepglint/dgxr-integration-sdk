using System;
using System.Collections;
using System.Collections.Generic;
using Moat;
using Moat.Model;
using UnityEngine;

public class RoamTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!DisplayData.wsConnect)
        {
            DebugPanel.Instance.BeControlledUserID = ((Int32.Parse("1001")) % 1000).ToString(); 
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Deepglint.XR.Toolkit.Utils;
using Samples.Cave.Data;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    public static CaveConfig Config { get; private set; }
    void Start()
    {
        Config = DataUtil.LoadData<CaveConfig>("Assets/Samples/Cave/Data/Resources/");
        Debug.Log(Config.Name);
    }
}

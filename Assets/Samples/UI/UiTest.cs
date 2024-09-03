using System;
using System.Collections;
using System.Collections.Generic;
using Deepglint.XR;
using Deepglint.XR.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "UI")
        {
            Debug.Log(111);
            DGXR.ApplicationSettings.toolkit.ExitButtonConfig.OnExit +=  OnExit;
            DGXR.ApplicationSettings.toolkit.ExitButtonConfig.ButtonText = "聚焦";
            DGXR.ApplicationSettings.toolkit.ExitButtonConfig.ExitingInfo = "聚焦成功";
                
        }
        else
        {
            Debug.Log(222);
            DGXR.ApplicationSettings.toolkit.ExitButtonConfig.OnExit =  null;
            DGXR.ApplicationSettings.toolkit.ExitButtonConfig.ButtonText = "退出";
            DGXR.ApplicationSettings.toolkit.ExitButtonConfig.ExitingInfo = "退出成功";
        }
        
        // DGXR.ApplicationSettings.toolkit.ExitButtonConfig.OnExiting +=  OnExiting;
        // DGXR.ApplicationSettings.toolkit.ExitButtonConfig.OnExit +=  OnExit;
        // DGXR.ApplicationSettings.toolkit.ExitButtonConfig.ButtonText = "聚焦";
        // DGXR.ApplicationSettings.toolkit.ExitButtonConfig.ExitingInfo = "聚焦成功";
    }

    // Update is called once per frame
    void OnExiting(int time)
    {
        Debug.Log("Exiting " + time);
    }

    private void OnExit()
    {
        SceneManager.LoadScene("UI 1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene("UI");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("UI 1");
        }
    }
}

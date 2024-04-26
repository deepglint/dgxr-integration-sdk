using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFactory
{
    private static PlayerFactory instance;
    public GameObject playerPerfab;
    
    public PlayerFactory()
    {
        playerPerfab = Resources.Load<GameObject>("Perfabs/Player");

        if (playerPerfab == null)
        {
            Debug.LogError("Player Prefab not found!");
        }
    }
        
    // 获取GameManager的实例
    public static PlayerFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerFactory();
            }
            return instance;
        }
    }

    public GameObject Create(PlayerInput input)
    {
        if (playerPerfab == null)
        {
            Debug.Log("no player perfab");
            return null;
        }
        Vector3 position = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        GameObject e = GameObject.Instantiate(playerPerfab.gameObject);
        // GameObject e = PlayerInput.Instantiate(playerPerfab, controlScheme: input.currentControlScheme,
        //     pairWithDevice: input.devices[0]);
        //PlayerInput playerInputInstance = PlayerInput.Instantiate(playerPerfab, -1, null, -1, null);
        // 获取PlayerInput实例的GameObject
        //GameObject e = playerInputInstance.gameObject;
        e.transform.position = position;
        Debug.Log("a player was created");
        return e;
    }
    
}

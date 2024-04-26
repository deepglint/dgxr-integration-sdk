// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class PlayerInputManagerScript1 : MonoBehaviour
// {
//     [SerializeField] private GameObject playerPrefab;  // Assign this in the inspector with your Player prefab
//     [SerializeField] private InputActionAsset inputActions;
//     private void Awake()
//     {
//         var playerInputManager = gameObject.AddComponent<PlayerInputManager>();
//         playerPrefab = Resources.Load<GameObject>("Perfabs/Player");
//         if (playerPrefab == null)
//         {
//             Debug.LogError("Player perfab not found");
//         }
//         playerInputManager.playerPrefab = playerPrefab;
//
//         inputActions = Resources.Load<InputActionAsset>("Actions/DGXR");
//         if (inputActions == null)
//         {
//             Debug.LogError("DGXR action assets not found");
//         }
//
//         var joinAction = inputActions.FindActionMap("DGXRDevice").FindAction("Join");
//         if (joinAction != null)
//         {
//             joinAction.Enable();
//             // 设置PlayerInputManager的joinAction属性
//             var joinActionProperty = new InputActionProperty(joinAction);
//             playerInputManager.joinAction = joinActionProperty;
//             playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
//         }
//         else
//         {
//             Debug.LogError("Join action not found in the Input Actions asset.");
//         }
//
//         
//         playerInputManager.JoinPlayer();
//         playerInputManager.onPlayerJoined += OnPlayerJoined;
//         playerInputManager.onPlayerLeft += OnPlayerLeft;
//     }
//     
//     private void OnDisable()
//     {
//         var playerInputManager = GetComponent<PlayerInputManager>();
//         //playerInputManager.onPlayerJoined -= OnPlayerJoined;
//         //playerInputManager.onPlayerLeft -= OnPlayerLeft;
//     }
//     
//     public void OnPlayerJoined(PlayerInput playerInput)
//     {
//         
//         // This is called when a player joins the game
//         // You can initialize player settings here
//         
//         Debug.Log("Player joined: " + playerInput.playerIndex);
//     }
//
//     public void OnPlayerLeft(PlayerInput playerInput)
//     {
//         // This is called when a player leaves the game
//         // You can handle player removal here
//         
//         Debug.Log("Player left: " + playerInput.playerIndex);
//     }
// }

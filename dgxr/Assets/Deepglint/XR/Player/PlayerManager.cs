using System;
using System.Collections.Generic;
using Deepglint.XR.Inputs;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Deepglint.XR.Player
{
    /// <summary>
    /// Determines how to join a player.
    /// </summary>
    public enum PlayerJoinBehaviour
    {
        /// <summary>
        /// Join a player when action is performed.
        /// </summary>
        JoinFromAction,
        
        /// <summary>
        /// Join a new player based on input on a UI element.
        /// </summary>
        /// <remarks>
        /// This should be called directly from a UI callback such as <see cref="Button.onClick"/>. The device
        /// that the player joins with is taken from the device that was used to interact with the UI element.
        /// </remarks>
        JoinFromUI,
    }
    
    /// <summary>
    /// Manages joining and leaving of players.
    /// </summary>
    public class PlayerManager : MonoBehaviour 
    {
        /// <summary>
        /// Prefab that the manager will instantiate when players join.
        /// </summary>
        [SerializeField] 
        private GameObject playerPrefab; 
       
        [SerializeField] 
        private PlayerJoinBehaviour m_JoinBehavior;

        private bool _enableJoining;
        
        /// <summary>
        /// The input action that a player must trigger to join the game.
        /// </summary>
        /// <remarks>
        /// If the join action is a reference to an existing input action, it will be cloned when the PlayerManager
        /// is enabled. This avoids the situation where the join action can become disabled after the first user joins which
        /// can happen when the join action is the same as a player in-game action. When a player joins, input bindings from
        /// devices other than the device they joined with are disabled. If the join action had a binding for keyboard and one
        /// for gamepad for example, and the first player joined using the keyboard, the expectation is that the next player
        /// could still join by pressing the gamepad join button. Without the cloning behavior, the gamepad input would have
        /// been disabled.
        /// </remarks> 
        [SerializeField]
        public InputActionProperty joinAction;
        
        [SerializeField]
        public GameObject joinUI;
        
        /// <summary>
        /// Manages all the playerInputs
        /// </summary>
        private static PlayerInputManager _playerInputManager;
        
        /// <summary>
        /// All the active PlayerInput count managed by the PlayerManager instance
        /// </summary>
        private static int _allActivePlayersCount;
        
        /// <summary>
        /// All the active PlayerInputs managed by the PlayerManager instance 
        /// </summary>
        private static PlayerInput[] _allActivePlayers;

        private static Dictionary<int, ICharacter> _characters = new Dictionary<int, ICharacter>();
        private static Dictionary<int, PlayerInput> _players = new Dictionary<int, PlayerInput>();
        
        public GameObject PlayerPrefab
        {
            get => playerPrefab;
            set => playerPrefab = value;
        }

        /// <summary>
        /// Singleton instance of the manager.
        /// </summary>
        public static PlayerManager Instance { get; private set; }
        
        /// <summary>
        /// Callback array to save all the OnTryToJoin delegate 
        /// </summary>
        private CallbackArray<Func<InputDevice, object>> _tryToJoinDelegate;
        
        private CallbackArray<Func<PlayerInput, InputDevice, object>> _playerJoinDelegate;
        
        public event Func<PlayerInput, InputDevice, ICharacter> OnTryToJoinWithICharacter
        {
            add
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _playerJoinDelegate.AddCallback(value);
            }
            remove
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _playerJoinDelegate.RemoveCallback(value);
            }
        }
        
        public event Func<InputDevice, Character> OnTryToJoinWithCharacter
        {
            add
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _tryToJoinDelegate.AddCallback(value);
            }
            remove
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _tryToJoinDelegate.RemoveCallback(value);
            }
        }
        
        private void Awake()
        {
            if (_playerInputManager == null)
            {
                _playerInputManager = gameObject.AddComponent<PlayerInputManager>();
                _playerInputManager.playerPrefab = playerPrefab;
                _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        
                _playerInputManager.onPlayerJoined += OnPlayerJoined;
                _playerInputManager.onPlayerLeft += OnPlayerLeft;
                DeviceManager.OnDeviceLost += OnDeviceRemoved;
            }
            // todo check player prefab exist;
            // if (playerPrefab == null)
            // {
            //     playerPrefab = Resources.Load<GameObject>("Perfabs/Player");
            //     if (playerPrefab == null)
            //     {
            //         Debug.LogError("Player perfab not found");
            //     }
            // }
        }

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple PlayerManagers in the game. There should only be one PlayerManager", this);
                return;
            }
        
            EnableJoining();
        }
        
        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            
            // _playerInputManager.DisableJoining();
            // InputUser.onChange -= OnInputUserChange;
            
            DisableJoining();
        }

        /// <summary>
        /// Allow players to join the game based on <see cref="PlayerJoinBehaviour"/>.
        /// </summary>
        /// <seealso cref="DisableJoining"/>
        public void EnableJoining()
        {
            switch (m_JoinBehavior)
            {
                case PlayerJoinBehaviour.JoinFromAction:
                    // if the join action is a reference, clone it so we don't run into problems with the action being disabled by
                    // PlayerInput when devices are assigned to individual players
                    if (joinAction.reference != null && joinAction.action?.actionMap?.asset != null)
                    {
                        var inputActionAsset = Instantiate(joinAction.action.actionMap.asset);
                        var inputActionReference = InputActionReference.Create(inputActionAsset.FindAction(joinAction.action.name));
                        joinAction = new InputActionProperty(inputActionReference);
                        joinAction.action.performed += OnJoinActionPerformed;
                        joinAction.action.Enable();
                    }          
                    _playerInputManager.EnableJoining();
                    
                    break;
                case PlayerJoinBehaviour.JoinFromUI:
                    if (joinUI == null)
                    {
                        Debug.LogError("joinUI should not be null");
                        break;
                    }
                    EventTrigger trigger = joinUI.GetComponent<EventTrigger>();
                    if (trigger == null)
                    {
                        trigger = joinUI.AddComponent<EventTrigger>();
                    }

                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter;
                    entry.callback.AddListener((eventData) =>
                    {
                        OnJoinUIEventTriggered((PointerEventData)eventData);
                    });

                    trigger.triggers.Add(entry);
                    Debug.LogFormat("Add OnPointerEnter listener to {0}", joinUI.name);
                    
                    break;
            }
            
            InputUser.onChange += OnInputUserChange;
            _enableJoining = true;
        }

        /// <summary>
        /// Inhibit players from joining the game.
        /// </summary>
        /// <seealso cref="EnableJoining"/>
        public void DisableJoining()
        {
            switch (m_JoinBehavior)
            {
                case PlayerJoinBehaviour.JoinFromAction:
                    _playerInputManager.DisableJoining();
                    break;
                case PlayerJoinBehaviour.JoinFromUI:
                    EventTrigger trigger = null;
                    if (joinUI != null)
                    {
                        trigger = joinUI.GetComponent<EventTrigger>();
                    }
                    if (trigger != null)
                    {
                        // 查找并移除 PointerEnter 监听器
                        for (int i = trigger.triggers.Count - 1; i >= 0; i--)
                        {
                            if (trigger.triggers[i].eventID == EventTriggerType.PointerEnter)
                            {
                                trigger.triggers.RemoveAt(i);
                            }
                        }
                    }
                    break;
            }
            
            InputUser.onChange -= OnInputUserChange;
            _enableJoining = false;
        }

        /// <summary>
        /// Determines the mechanism by which players can join. 
        /// </summary>
        public PlayerJoinBehaviour JoinBehavior
        {
            get => m_JoinBehavior;
            set
            {
                if (m_JoinBehavior == value)
                    return;
                var enabled = _enableJoining;
                if (enabled)
                {
                    DisableJoining();
                }
                m_JoinBehavior = value;
                if (enabled)
                {
                    EnableJoining();
                }
            }
        }

        /// <summary>
        /// The devices paired to all the players managed by PlayerManager.
        /// </summary>
        /// <value>List of devices paired to all the players.</value>
        /// <remarks>
        /// </remarks>
        public ReadOnlyArray<InputDevice> AllPairedDevices
        {
            get
            {
                InputDevice[] devices = null;
                for (var i = 0; i < _allActivePlayersCount; ++i)
                {
                    ArrayHelper.Append(ref devices, _allActivePlayers[i].devices);
                }

                return devices;
            }
        }

        private void OnJoinBehaviorPerformed(InputDevice device)
        {
            if (!CheckIfPlayerCanJoin())
            {
                return;
            }
            
            if (PlayerInput.FindFirstPairedToDevice(device) != null)
            {
                // forbidden to pair a device to multi player on joining stage.
                return;
            }
            
            // PlayerInput playerInput = null;
            var obj = DelegateHelper.InvokeCallbacksSafe_AnyCallbackReturnsObject(
                ref _tryToJoinDelegate, device, "PlayerManager.onTryToJoin");
            if (obj != null)
            {
                if (obj is Character character)
                {
                    if (PairDeviceToCharacter(character, device))
                    {
                        Debug.LogFormat("succeed to bind device {0} to player {1}", device.deviceId, character.Name);
                    }
                    else
                    {
                        Debug.LogFormat("failed to bind device {0} to player {1}", device.deviceId, character.Name);
                    }
                }
            }
            else
            {
                // Initiate a new player and pair the device to the new player.
                var playerInput = _playerInputManager.JoinPlayer(pairWithDevice: device);
                if (playerInput != null)
                {
                    obj = DelegateHelper.InvokeCallbacksSafe_AnyCallbackReturnsObject(
                        ref _playerJoinDelegate, playerInput, device, "PlayerManager.onTryToJoin");
                    if (obj != null)
                    {
                        playerInput.gameObject.AddComponent<PlayerGarbageCollector>();
                        _characters.Add(device.deviceId, (ICharacter)obj);
                        _players.Add(device.deviceId, playerInput);
                        Debug.LogFormat("player {0} which paired to {1} joined with ICharacter succeed", playerInput.user.id, device.deviceId);
                    }
                    else
                    {
                        playerInput.user.UnpairDevices();
                        playerInput.gameObject.SetActive(false);
                        // todo Destroy(playerInput.gameObject);
                    }
                }
                
            }
        }
        
        private void OnJoinActionPerformed(InputAction.CallbackContext context)
        {
            OnJoinBehaviorPerformed(context.control.device);
        }

        private void OnJoinUIEventTriggered(PointerEventData eventData)
        {
            var device = DeviceManager.GetActiveDeviceById(Math.Abs(eventData.pointerId));
            if (device != null)
            {
                OnJoinBehaviorPerformed(device);
            }
        }

        public bool PairDeviceToPlayerManually(PlayerInput pi, InputDevice device)
        {
            if (pi != null)
            {
                // todo forbidden pair duplicate DGXRController device to one player.
                // foreach (var pairedDevice in pi.devices)
                // {
                //     if (pairedDevice is DGXRHumanController)
                //     {
                //        
                //         return false;
                //     }
                // }

                // Pair the device to the given player
                InputUser.PerformPairingWithDevice(device, pi.user);
                return true;
            }
            
            return false;
        }

        public void UnpairDeviceFromPlayerManually(PlayerInput pi, InputDevice device)
        {
            if (pi != null)
            {
                pi.user.UnpairDevice(device);
            }
        }

        internal void DestroyPlayer(PlayerInput pi)
        {
            if (pi == null)
            {
                return;
            }
            foreach (var device in pi.devices)
            {
                if (_characters.TryGetValue(device.deviceId, out var iCharacter))
                {
                    iCharacter.OnPlayerLeft();
                    _characters.Remove(device.deviceId);
                    Debug.LogFormat("player on character {0} was destroyed", iCharacter.GetHashCode());
                }
            }
        }

        /// <summary>
        /// pair the device the given character.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="pairDevice"></param>
        /// <returns></returns>
        internal bool PairDeviceToCharacter(Character character, InputDevice pairDevice)
        {
            PlayerInput playerInput = null;
            if (character.m_Player is null)
            {
                // Initiate a new player for the character and pair the device to the new player.
                playerInput = _playerInputManager.JoinPlayer(pairWithDevice: pairDevice);
                Player player = playerInput.gameObject.AddComponent<Player>();
                character.m_Player = player;
                player.m_Character = character;
                player.m_PlayerInput = playerInput;
                Debug.LogFormat("player {0} joined with character {1}", playerInput.user.id, character.Name);
                return true;
            }

            playerInput = character.m_Player.m_PlayerInput;
            if (playerInput is not null)
            {
                // Pair the device to the player which controls current character.
                InputUser.PerformPairingWithDevice(pairDevice, playerInput.user);
                return true;
            }

            return false;
        }

        private bool CheckIfPlayerCanJoin(int playerIndex = -1)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("playerPrefab must be set in order to be able to join new players", this);
                return false;
            }

            return true;
        }
    
        /// <summary>
        /// callback invoked when a PlayerInput component is enabled.
        /// </summary>
        /// <param name="playerInput"></param>
        private void OnPlayerJoined(PlayerInput playerInput)
        {
            ArrayHelper.AppendWithCapacity<PlayerInput>(ref _allActivePlayers, ref _allActivePlayersCount, playerInput);
            Debug.Log("Player joined: " + playerInput.playerIndex);
        }

        /// <summary>
        /// callback invoked when a PlayerInput component is disabled.
        /// </summary>
        /// <param name="playerInput"></param>
        private void OnPlayerLeft(PlayerInput playerInput)
        {
            var index = ArrayHelper.IndexOfReference(_allActivePlayers, playerInput, _allActivePlayersCount);
            if (index != -1)
            {
                ArrayHelper.EraseAtWithCapacity(_allActivePlayers, ref _allActivePlayersCount, index);
            }
            Debug.LogFormat("Player {0} left, and current paired device count is {1}", playerInput.playerIndex, playerInput.devices.Count);
        }

        /// <summary>
        /// callback invoked when a paired device is lost. 
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="device"></param>
        private static void OnDeviceLost(PlayerInput pi, InputDevice device)
        {
            pi.user.UnpairDevice(device);
            if (_characters.TryGetValue(device.deviceId, out var iCharacter))
            {
                iCharacter.OnPlayerLeft();
                _characters.Remove(device.deviceId);
            }
            Debug.LogFormat("device {0} was unpaired from player {1}, current player device count is {2}", device.deviceId, pi.user.id, pi.devices.Count);
        }

        private static void OnDeviceRemoved(int deviceId)
        {
            if (_players.TryGetValue(deviceId, out var pi))
            {
                _players.Remove(deviceId);
                Destroy(pi.gameObject);
            }
        }
        
        /// <summary>
        /// callback invoked when a paired device is regained. 
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="device"></param>
        private static void OnDeviceRegained(PlayerInput pi, InputDevice device)
        {
            Debug.LogFormat("device {0} was regained", device.deviceId);
        }

        /// <summary>
        /// callback invoked when a device is paired to a player
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="device"></param>
        private static void OnDevicePaired(PlayerInput pi, InputDevice device)
        {
            Debug.LogFormat("device {0} was paired to player {1}", device.deviceId, pi.user.id);
            // double check: it is forbidden to pair more than one main device to on player;
            if (pi.devices.Count > 1)
            {
                Debug.LogFormat("unpair device {0} from player {1}, cause over size", device.deviceId, pi.user.id);
                pi.user.UnpairDevice(device);
            } 
        }

        /// <summary>
        /// callback invoked when a InputUser is changed.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="change"></param>
        /// <param name="device"></param>
        private static void OnInputUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            switch (change)
            {
                case InputUserChange.DeviceLost:
                    for (var i = 0; i < _allActivePlayersCount; ++i)
                    {
                        var player = _allActivePlayers[i];
                        if (player.user == user)
                        {
                            OnDeviceLost(player, device);
                        }
                    }
                    
                    break;
                case InputUserChange.DeviceRegained:
                    for (var i = 0; i < _allActivePlayersCount; ++i)
                    {
                        var player = _allActivePlayers[i];
                        if (player.user == user)
                        {
                            OnDeviceRegained(player, device); 
                        }
                    }

                    break;
                case InputUserChange.DevicePaired:
                    for (var i = 0; i < _allActivePlayersCount; ++i)
                    {
                        var player = _allActivePlayers[i];
                        if (player.user == user)
                        {
                            OnDevicePaired(player, device); 
                        }
                    } 
                    
                    break;
            }
        }

        private void OnDestroy()
        {
            DeviceManager.OnDeviceLost -= OnDeviceRemoved;
            InputUser.onChange -= OnInputUserChange;
            _playerInputManager.onPlayerJoined -= OnPlayerJoined;
            _playerInputManager.onPlayerLeft -= OnPlayerLeft;
            joinAction.action.performed -= OnJoinActionPerformed;
            Instance = null;
        }
    }
}

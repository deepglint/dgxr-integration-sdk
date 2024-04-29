using System;
using Deepglint.XR.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace Deepglint.XR
{
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
        private InputActionProperty joinAction;
        
        /// <summary>
        /// Manages all the playerInputs
        /// </summary>
        private PlayerInputManager _playerInputManager;
        
        /// <summary>
        /// All the active PlayerInput count managed by the PlayerManager instance
        /// </summary>
        private static int _allActivePlayersCount;
        
        /// <summary>
        /// All the active PlayerInputs managed by the PlayerManager instance 
        /// </summary>
        private static PlayerInput[] _allActivePlayers;

        /// <summary>
        /// Singleton instance of the manager.
        /// </summary>
        public static PlayerManager Instance { get; private set; }
        
        /// <summary>
        /// Callback array to save all the OnTryToJoin delegate 
        /// </summary>
        private static CallbackArray<Func<InputDevice, object>> _tryToJoinDelegate;
        
        public static event Func<InputDevice, Character> OnTryToJoin
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
            _playerInputManager = gameObject.AddComponent<PlayerInputManager>();
            // todo check player prefab exist;
            // if (playerPrefab == null)
            // {
            //     playerPrefab = Resources.Load<GameObject>("Perfabs/Player");
            //     if (playerPrefab == null)
            //     {
            //         Debug.LogError("Player perfab not found");
            //     }
            // }
            _playerInputManager.playerPrefab = playerPrefab;
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
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

            InputUser.onChange += OnInputUserChange;
        }
        
        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            _playerInputManager.DisableJoining();
            InputUser.onChange -= OnInputUserChange;
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

        private void OnJoinActionPerformed(InputAction.CallbackContext context)
        {
            if (!CheckIfPlayerCanJoin())
            {
                return;
            }

            var device = context.control.device;
            if (PlayerInput.FindFirstPairedToDevice(device) != null)
            {
                // forbidden to pair a device to multi player.
                return;
            }

            // PlayerInput playerInput = null;
            var obj = DelegateHelper.InvokeCallbacksSafe_AnyCallbackReturnsObject(
                ref _tryToJoinDelegate, device, "PlayerManager.onTryToJoin");
            if (obj is Character character)
            {
                Debug.LogFormat("trying to bind device {0} to player {1}", device.deviceId, character.Name);
                if (PairDeviceToCharacter(character, device))
                {
                    Debug.LogFormat("succeed to bind device {0} to player {1}", device.deviceId, character.Name);
                }
                else
                {
                    Debug.LogFormat("failed to bind device {0} to player {1}", device.deviceId, character.Name);
                }
            }
            else
            {
                Debug.LogFormat("device {0} join failed", device.deviceId);
            }
        }

        /// <summary>
        /// pare the device the given character.
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

            // todo control player count;

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
            Debug.LogFormat("device {0} was unpaired from player {1}, current player device count is {2}", device.deviceId, pi.user.id, pi.devices.Count);
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
    }
}

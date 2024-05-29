using System;
using Deepglint.XR;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// This demo demonstrates the process of multi player management.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager4 : MonoBehaviour
    {
        private Character4 _character;
        [SerializeField] 
        private GameObject _circlePrefab;

        private GameObject ui;

        public void Awake()
        {
            if (_circlePrefab == null)
            {
                Debug.LogError("_circlePrefab not set");
            }
        }

        public void Start()
        {
            RectTransform parentRT = Global.Space.Bottom.ScreenCanvas.GetComponent<RectTransform>();
            ui = Instantiate(_circlePrefab, parentRT, false);
            ui.SetActive(true);
            RectTransform circleRT = ui.GetComponent<RectTransform>();
            circleRT.localPosition = Vector3.zero;
            circleRT.sizeDelta = new Vector2(300, 300);
            circleRT.localScale = Vector3.one;
            
            _character = new Character4("UICharacter", ui);
            
            PlayerManager.Instance.joinUI = ui;
            PlayerManager.Instance.JoinBehavior = PlayerJoinBehaviour.JoinFromUI;
            PlayerManager.OnTryToJoinWithICharacter += _character.OnTryToJoin;
        }
    
        public class Character4 : ICharacter
        {
            private readonly GameObject _characterUI;
            private GameObject _player;
            private PlayerInput _playerInput;

            public GameObject Player => _player;
            
            public readonly string Name;
            
            public Character4(string name, GameObject ui)
            {
                Name = name;
                _characterUI = ui;
                EventTrigger trigger = _characterUI.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = _characterUI.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener((eventData) =>
                {
                    Left((PointerEventData)eventData);
                });

                trigger.triggers.Add(entry);
            }

            public ICharacter OnTryToJoin(InputDevice device)
            {
                if (_playerInput is null)
                {
                    if (device is DGXRHumanController)
                    {
                        Debug.LogFormat("character {0} is bindable", Name);
                        return this;
                    }
                }
                else
                {
                    if (device is DGXRHumanController && _playerInput.devices.Count == 0)
                    {
                        Debug.LogFormat("character {0} is bindable", Name);
                        _player.SetActive(true);
                        PlayerManager.Instance.PairDeviceToPlayer(_player, device);
                        Debug.LogFormat("device {0} is paired to character {1}", device.deviceId, Name);
                        return null;
                    }
                }

                Debug.LogFormat("character {0} is not bindable", Name);
                return null;
            }

            public void Join(GameObject player)
            {
                _player = player;
                _playerInput = player.GetComponent<PlayerInput>();
            }

            public void Left(PointerEventData eventData)
            {
                var device = (DGXRHumanController)DeviceManager.GetActiveDeviceById(Math.Abs(eventData.pointerId));
                if (device == null)
                {
                    return;
                }

                var position = device.HumanPose.Position.value;
                var screenPoint = new Vector2(position.x * Global.Space.Bottom.Resolution.width / Global.Space.Bottom.Size.x, 
                    position.z * Global.Space.Bottom.Resolution.width / Global.Space.Bottom.Size.y);
                if (!RectTransformUtility.RectangleContainsScreenPoint(_characterUI.GetComponent<RectTransform>(), screenPoint))
                {
                    if (_playerInput.devices.Count > 0) 
                    {
                        PlayerManager.Instance.UnpairDeviceFromPlayer(_player, device);
                        Debug.LogFormat("device {0} is unpaired from character {1}", device.deviceId, Name);
                        _player.SetActive(false);
                    }
                }
            }
        }
    }
}
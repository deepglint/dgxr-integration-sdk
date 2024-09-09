using Deepglint.XR;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

// This demo demonstrates the process of player joining, manually unbinding, and manually rebinding.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager1 : MonoBehaviour
    {
        private CustomCharacter1 _character;

        public void Start()
        {
            _character = new CustomCharacter1("领航员", new ROI(){ Anchor = Vector2.zero, Radius = 1.0f });
            PlayerManager.Instance.OnTryToJoinWithCharacter += _character.OnTryToJoin;
        }

        public void Update()
        {
            if (_character.Player is not null)
            {
                // check and unpair device manually
                foreach (var device in _character.Player.PairedDevices)
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(_character.Roi.Anchor, new Vector2(position.x, position.z)) > _character.Roi.Radius)
                        {
                            DGXR.Logger.Log($"device {device.deviceId} stepped out from {_character.Name}'s roi");
                            _character.Player.UnPairDeviceManually(device);
                            DGXR.Logger.Log($"unpair device {device.deviceId} from character {_character.Name} manually");
                        }
                    }
                    else
                    {
                        DGXR.Logger.Log($"device {device.deviceId} is not dgxr device");
                    }
                }

                // check and pair device manually
                if (_character.Player.PairedDevices.Count == 0)
                {
                    var devices = DeviceManager.AllActiveXRHumanDevices;
                    var allPairedDevices = PlayerManager.Instance.AllPairedDevices.ToArray();
                    foreach (var device in devices)
                    {
                        if (!ArrayHelper.Contains(allPairedDevices, device))
                        {
                            Vector3 position = device.HumanPose.Position.ReadValue();
                            if (Vector2.Distance(_character.Roi.Anchor, new Vector2(position.x, position.z)) < _character.Roi.Radius)
                            {
                                DGXR.Logger.Log($"device {device.deviceId} steeped into {_character.Name}'s roi");
                                if (_character.Player.PairDeviceManually(device))
                                {
                                    DGXR.Logger.Log($"pair device {device.deviceId} to character {_character.Name} manually ");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public struct ROI
        {
            public Vector2 Anchor;
            public float Radius;
        }

        public class CustomCharacter1 : Character
        {
            public ROI Roi;
            public CustomCharacter1(string name, ROI roi)
            {
                Name = name;
                Roi = roi;
            }
            public override Character OnTryToJoin(InputDevice device)
            {
                if (IsBindable())
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(Roi.Anchor,new Vector2(position.x, position.z)) <= Roi.Radius)
                        {
                            DGXR.Logger.Log("character {Name} is bindable");
                            return this;
                        }
                    }
                }

                DGXR.Logger.Log("character {Name} is not bindable");
                return null;
            }
        }
    }
}

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
                            Debug.LogFormat("device {0} stepped out from {1}'s roi", device.deviceId, _character.Name);
                            _character.Player.UnPairDeviceManually(device);
                            Debug.LogFormat("unpair device {0} from character {1} manually", device.deviceId, _character.Name);
                        }
                    }
                    else
                    {
                        Debug.LogFormat("device {0} is not dgxr device", device.deviceId);
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
                                Debug.LogFormat("device {0} steeped into {1}'s roi", device.deviceId, _character.Name);
                                if (_character.Player.PairDeviceManually(device))
                                {
                                    Debug.LogFormat("pair device {0} to character {1} manually ", device.deviceId, _character.Name);
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
                            Debug.LogFormat("character {0} is bindable", Name);
                            return this;
                        }
                    }
                }

                Debug.LogFormat("character {0} is not bindable", Name);
                return null;
            }
        }
    }
}

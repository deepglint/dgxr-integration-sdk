using UnityEngine;
using Deepglint.XR;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Utils;
using UnityEngine.InputSystem;

// This demo demonstrates the process of player joining, manually unbinding, and manually rebinding.
public class CustomPlayerManager1 : MonoBehaviour
{
    private CustomCharacter1 character;

    public void Start()
    {
        character = new CustomCharacter1("领航员", new ROI(){ Anchor = Vector2.zero, Radius = 1.0f }); 
        PlayerManager.OnTryToJoin += character.OnJoin;
    }

    public void Update()
    {
        if (character.Player is not null)
        {
            // check and unpair device manually
            foreach (var device in character.Player.PairedDevices)
            {
                if (device is DGXRController dgXRDevice)
                {
                    Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                    if (Vector2.Distance(character.roi.Anchor, new Vector2(position.x, position.z)) > character.roi.Radius)
                    {
                        Debug.LogFormat("device {0} stepped out from {1}'s roi", device.deviceId, character.Name);
                        character.Player.UnPairDeviceManually(device);
                        Debug.LogFormat("unpair device {0} from character {1} manually", device.deviceId, character.Name);
                    }
                }
                else
                {
                    Debug.LogFormat("device {0} is not dgxr device", device.deviceId);
                }
            }
            
            // check and pair device manually
            if (character.Player.PairedDevices.Count == 0)
            {
                var devices = DeviceManager.AllActiveDevices;
                var allPairedDevices = PlayerManager.Instance.AllPairedDevices.ToArray();
                foreach (var device in devices)
                {
                    if (!ArrayHelper.Contains(allPairedDevices, device) &&  device is DGXRController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(character.roi.Anchor, new Vector2(position.x, position.z)) < character.roi.Radius)
                        {
                            Debug.LogFormat("device {0} steeped into {1}'s roi", device.deviceId, character.Name); 
                            if (character.Player.PairDeviceManually(device))
                            {
                                Debug.LogFormat("pair device {0} to character {1} manually ", device.deviceId, character.Name);
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
        public ROI roi;
        public CustomCharacter1(string name, ROI roi)
        {
            Name = name;
            this.roi = roi;
        }
        public override Character OnJoin(InputDevice device)
        {
            if (IsBindable())
            {
                if (device is DGXRController dgXRDevice)
                {
                    Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                    if (Vector2.Distance(roi.Anchor,new Vector2(position.x, position.z)) <= roi.Radius)
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

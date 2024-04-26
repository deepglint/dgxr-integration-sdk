using UnityEngine;
using Deepglint.XR;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Utils;
using UnityEngine.InputSystem;

// 无限模式
public class CustomPlayerManager3 : MonoBehaviour
{
    private CustomPlayerInfo3 info;

    public void Start()
    {
        info = new CustomPlayerInfo3("滑雪运动员", new Vector2(0, 0), 1f); 
        PlayerManager.OnTryToJoin += info.OnJoin;
    }

    public void Update()
    {
        if (info.m_player != null && info.IsBindable())
        {
            var devices = DeviceManager.AllActiveDevices;
            var pairedDevices = PlayerManager.instance.AllPairedDevices.ToArray();
            foreach (var device in devices)
            {
                if (!ArrayHelper.Contains(pairedDevices, device) &&  device is DGXRController dgXRDevice)
                {
                    Vector2 position = dgXRDevice.HumanPose.position.ReadValue();
                    if (Vector2.Distance(info.roiAnchor, position) <= info.roiRadio)
                    {
                        if (info.Player.PairDeviceManually(device))
                        {
                            Debug.LogFormat("repair player {0} to device {1}", info.Name, device.deviceId);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class CustomPlayerInfo3 : Character
    {
        public Vector2 roiAnchor;
        public float roiRadio;
        public CustomPlayerInfo3(string name, Vector3 anchor, float radio)
        {
            this.Name = name;
            roiAnchor = anchor;
            roiRadio = radio;
        }
        public override Character OnJoin(InputDevice device)
        {
            Debug.LogFormat("trying to bind to player: " + Name);
            if (IsBindable())
            {
                if (device is DGXRController dgXRDevice)
                {
                    Vector2 position = dgXRDevice.HumanPose.position.ReadValue();
                    if (Vector2.Distance(roiAnchor,position) <= roiRadio)
                    {
                        Debug.LogFormat("player {0} is bindable", Name);
                        return this; 
                    }
                }
            }
            Debug.LogFormat("player {0} is not bindable", this.Name);
            return null;
        }
    }
}

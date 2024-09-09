using Deepglint.XR;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Samples.Cave
{
    public class CavePlayerManager : MonoBehaviour
    {
        private VideoWatcherCharacter _videoWatcher;

        private void Start()
        {
            _videoWatcher = new VideoWatcherCharacter("videoWatcher");
            PlayerManager.Instance.OnTryToJoinWithCharacter += _videoWatcher.OnTryToJoin;
        }

        private void Update()
        {
            if (_videoWatcher.Player is not null)
            {
                if (_videoWatcher.Player.PairedDevices.Count > 0)
                {
                    foreach (var device in _videoWatcher.Player.PairedDevices)
                    {
                        if (device is DGXRHumanController dgXRDevice)
                        {
                            Vector3 position = dgXRDevice.HumanBody.HeadTop.position.ReadValue();
                            DGXR.CavePosition = position;
                        }
                    }
                }
                else
                {
                    DGXR.CavePosition = new Vector3(0, 1.6f, 0);
                }

            }
        }

        public class VideoWatcherCharacter : Character
        {
            public VideoWatcherCharacter(string name)
            {
                Name = name;
            }

            public override Character OnTryToJoin(InputDevice device)
            {
                if (!IsBindable())
                {
                    foreach (var playerPairedDevice in Player.PairedDevices)
                    {
                        Player.UnPairDeviceManually(playerPairedDevice);
                    }
                }

                return this;
            }
        }
    }
}

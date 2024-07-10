using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;

// This demo demonstrates the process of multi player management.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager3 : MonoBehaviour
    {
        private Character3 _character;

        public void Start()
        {
            _character = new Character3("火柴人", new ROI(){ Anchor = new Vector2(0, 0), Radius = 1.0f });
            PlayerManager.Instance.OnTryToJoinWithICharacter += _character.OnPlayerJoin;
        }

        public struct ROI
        {
            public Vector2 Anchor;
            public float Radius;
        }

        public class Character3 : ICharacter
        {
            private readonly ROI _roi;
            private GameObject _player;

            public GameObject Player => _player;

            public readonly string Name;

            public Character3(string name, ROI roi)
            {
                Name = name;
                _roi = roi;
            }

            public ICharacter OnPlayerJoin(GameObject player, InputDevice device)
            {
                if (_player is null)
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(_roi.Anchor,new Vector2(position.x, position.z)) < _roi.Radius)
                        {
                            Debug.LogFormat("character {0} is bindable", Name);
                            _player = player;
                            return this;
                        }
                    }
                }

                Debug.LogFormat("character {0} is not bindable", Name);
                return null;
            }

            public void OnPlayerLeft()
            {
                Debug.LogFormat("player {0} is left", Name);
                _player = null;
            }
        }
    }
}
using System;
using DeepGlint.XR.Inputs.Devices;
using DeepGlint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;

// This demo demonstrates the process of multi player management.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager3 : MonoBehaviour
    {
        private Character3 _matchStickMan;

        public void Update()
        {
        }

        public void Start()
        {
            _matchStickMan = new Character3("火柴人", new ROI(){ Anchor = new Vector2(0, 0), Radius = 1.0f }); 
            PlayerManager.OnTryToJoinWithICharacter += _matchStickMan.OnTryToJoin;
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
        
            public ICharacter OnTryToJoin(InputDevice device)
            {
                if (_player is null)
                {
                    if (device is DGXRController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(_roi.Anchor,new Vector2(position.x, position.z)) < _roi.Radius)
                        {
                            Debug.LogFormat("character {0} is bindable", Name);
                            return this; 
                        }
                    }
                }
                Debug.LogFormat("character {0} is not bindable", Name);
                return null;
            }

            public void Join(GameObject player)
            {
                _player = player;
            }
        }
    }
}
using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;

// This demo demonstrates the process of multi player management.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager2 : MonoBehaviour
    {
        private List<Character2> _characters = new List<Character2>();
        private Character2 _character1;
        private Character2 _character2;
        private Character2 _character3;

        public void Start()
        {
            _characters.Add(new Character2("小红", new ROI(){ Anchor = new Vector2(-1, 0), Radius = 1.0f }));
            _characters.Add(new Character2("蓝蓝", new ROI(){ Anchor = Vector2.zero, Radius = 1.0f }));
            _characters.Add(new Character2("阿强", new ROI(){ Anchor = new Vector2(1, 0), Radius = 1.0f }));
            foreach (var character in _characters)
            {
                PlayerManager.Instance.OnTryToJoinWithCharacter += character.OnTryToJoin;
            }
        }

        public struct ROI
        {
            public Vector2 Anchor;
            public float Radius;
        }

        public class Character2 : Character
        {
            public ROI Roi;
            public Character2(string name, ROI roi)
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
                        if (Vector2.Distance(Roi.Anchor,new Vector2(position.x, position.z)) < Roi.Radius)
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

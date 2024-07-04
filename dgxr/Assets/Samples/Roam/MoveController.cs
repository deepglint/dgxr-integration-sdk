using System.Collections.Generic;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Toolkit.RoamStick;
using UnityEngine;
using UnityEngine.Serialization;

namespace Samples.Roam
{
    public struct HumanLocalPoseState
    {
        public Vector3 Move3DPos;
        public Vector2 Move2DPos;
        public float Angle;
    }
    
    public class MoveController: MonoBehaviour
    {
        private Dictionary<KeyCode, Vector3> _movementMappings =
            new Dictionary<KeyCode, Vector3>()
            {
                { KeyCode.W, new Vector3(0, 1, 1) },
                { KeyCode.S, new Vector3(0, -1, 1) },
                { KeyCode.A, new Vector3(-1, 0, 1) },
                { KeyCode.D, new Vector3(1, 0, 1) },
                { KeyCode.Q, new Vector3(2, 2, 1) },
                { KeyCode.E, new Vector3(3, 3, 1) },
                { KeyCode.Z, new Vector3(4, 4, 1) },
                { KeyCode.Alpha0, new Vector3(0, 0, 1) },
            };

        [FormerlySerializedAs("RealHuman")] public RealHumanEvent realHuman;
        [FormerlySerializedAs("RoamStick")] public RoamStick roamStick;
        private Vector2 _movePosition;

        private void Awake()
        {
            realHuman = gameObject.GetComponent<RealHumanEvent>();
            roamStick = GameObject.Find("RoamStick")?.GetComponent<RoamStick>();
        }

        public void UpdateMove(HumanPoseState humanPose, HumanLocalPoseState humanLocalPose)
        {
            if (roamStick == null || Samples.Roam.CharacterManager.MainCharacter == null) return;
            roamStick.humanBody = transform.gameObject;
            roamStick.Move(humanLocalPose.Move3DPos, humanLocalPose.Move2DPos, humanLocalPose.Angle);
        }

        public void ResetMove(bool isBackStart)
        {
            _movePosition = Vector2.zero;
            if (!isBackStart) return;
            roamStick.UpdateRoamMoveFromStick(Vector2.zero);
            transform.position = new Vector3(0, 3f, 0);
        }

        private void Update()
        {
            if (realHuman == null || realHuman.AppCharacter == null || roamStick == null) return; 

            if (realHuman.isRealHuman) return;
            realHuman.MockMovementPosition(_movePosition);

            foreach (var k in _movementMappings)
            {
                Vector2 target = new Vector2(k.Value.x, k.Value.y);
               
                if (target == Vector2.zero && Input.GetKeyDown(k.Key))
                {
                    if (!Input.GetKeyDown(k.Key) || realHuman.isRealHuman) continue;
                    realHuman.OnDeviceLost();
                    Destroy(gameObject);
                }
                else if (target == new Vector2(2, 2) && Input.GetKeyDown(k.Key))
                {
                    realHuman.RaiseOneHand();
                }
                else if (target == new Vector2(3, 3) && Input.GetKeyDown(k.Key))
                {
                    realHuman.RaiseBothHand();
                }
                else if (target == new Vector2(4, 4) && Input.GetKeyDown(k.Key))
                {
                    realHuman.Jump();
                }
                else if (Input.GetKeyDown(k.Key))
                {
                    _movePosition = target * 5f;
                }
                else if (Input.GetKeyUp(k.Key))
                {
                    _movePosition = Vector2.zero;
                }
            }
        }
    }
}
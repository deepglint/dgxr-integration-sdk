using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Moat;
using Moat.Model;

namespace Moat
{
    /// <summary>
    /// 虚拟设备控制类
    /// </summary>
    public class DeviceController : MonoBehaviour
    {
        private VirtualPlayer _player;
        private IMatchRule _matchRule;
        public GameObject PlayerBody;
        public Canvas PlayerCircle;

        [SerializeField] private int userId;

        private void Awake()
        {
            if (PlayerBody != null) PlayerBody.SetActive(false);
            if (PlayerCircle != null) PlayerCircle.gameObject.SetActive(false);
        }

        private void Start()
        {
            DisplayData.ReadConfig();
            userId = gameObject.GetComponent<PlayerInput>().playerIndex + 1;
            MDebug.Log("player-id: " + userId + " name: " + gameObject.GetComponent<PlayerInput>().name);
            _player = new VirtualPlayer(userId.ToString());
            _matchRule = new OnRaiseRightMatch(_player);
        }

        private void Update()
        {
            _matchRule.Update();
        }

        public bool OnRoam(Vector2 move)
        {
            if (CameraRoamControl.Instance != null && CameraRoamControl.Instance.isRoam)
            {
                CameraRoamControl.Instance.verticalInput = move.y;
                CameraRoamControl.Instance.horizontalInput = move.x;

                CancelInvoke("ResetMoveValue");
                Invoke("ResetMoveValue", 1f);
                return true;
            }

            return false;
        }

        private bool CheckListeningPermissions(string playerId)
        {
            if (DisplayData.configDisplay.interactionPermissionLevel >= 1)
            {
                Debug.Log("interactionPermissionLevel playerId: " + playerId + "--" +
                          PlayerGroup.Instance.GetVirtualPlayerById(playerId));
                return PlayerGroup.Instance.GetVirtualPlayerById(playerId) != null;
            }

            return true;
        }

        void ResetMoveValue()
        {
            CameraRoamControl.Instance.verticalInput = 0;
            CameraRoamControl.Instance.horizontalInput = 0;
        }

        public void OnMove(InputValue context)
        {
            //if (!context.started) return;
            if (!CheckListeningPermissions(_player.id)) return;
            float speed = 1f;
            Vector2 moveVec = context.Get<Vector2>() * speed;

            if (OnRoam(moveVec)) return;
            _player.movementInput += moveVec;
            _player.leftFootInput += moveVec;
            _player.rightFootInput += moveVec;
        }

        public void OnRightHandDrawCircle(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRightHandDrawCircle, new object[1] { playerID });
        }

        public void OnLeftHandDrawCircle(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeftHandDrawCircle, new object[1] { playerID });
        }

        public void OnFastRun(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnFastRun, new object[1] { playerID });
        }

        public void OnButterfly(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnButterfly, new object[1] { playerID });
        }

        public void OnApplaud(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnApplaud, new object[1] { playerID });
        }

        public void OnJump(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnJump, new object[1] { playerID });
        }

        public void OnRaiseOnHand(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            // if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRaiseOnHand, new object[1] { playerID });
        }

        public void OnSlideLeft(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideLeft, new object[1] { playerID });
        }

        public void OnSlideRight(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideRight, new object[1] { playerID });
        }

        public void OnSlideUp(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideUp, new object[1] { playerID });
        }

        public void OnSlideDown(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideDown, new object[1] { playerID });
        }

        public void OnWaving(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnWaving, new object[1] { playerID });
        }

        public void OnArmToForward(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(0, 1))) return;
            EventManager.Send(ActionEvent.OnArmToForward, new object[1] { playerID });
        }

        public void OnArmToBack(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(0, -1))) return;
            EventManager.Send(ActionEvent.OnArmToBack, new object[1] { playerID });
        }

        public void OnArmToLeft(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(-1, 0))) return;
            EventManager.Send(ActionEvent.OnArmToLeft, new object[1] { playerID });
        }

        public void OnArmToRight(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(1, 0))) return;
            EventManager.Send(ActionEvent.OnArmToRight, new object[1] { playerID });
        }

        public void OnHandsCross(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            // if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsCross, new object[1] { playerID });
        }

        public void OnPoseA(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseA, new object[1] { playerID });
        }

        public void OnPoseB(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseB, new object[1] { playerID });
        }

        public void OnPoseC(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseC, new object[1] { playerID });
        }

        public void OnPoseD(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseD, new object[1] { playerID });
        }

        public void OnLeanToLeft(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeanToLeft, new object[1] { playerID });
        }

        public void OnLeanToRight(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeanToRight, new object[1] { playerID });
        }

        public void OnSmallSquat(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSmallSquat, new object[1] { playerID });
        }

        // 补全剩下的配置
        public void OnHandBevelCut(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandBevelCut, new object[1] { playerID });
        }

        public void OnHandParry(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandParry, new object[1] { playerID });
        }

        public void OnHandStraightCut(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandStraightCut, new object[1] { playerID });
        }

        public void OnHandTransversal(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandTransversal, new object[1] { playerID });
        }

        public void OnStraightPunch(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnStraightPunch, new object[1] { playerID });
        }

        public void OnReadyStraightPunch(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyStraightPunch, new object[1] { playerID });
        }

        public void OnUppercut(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnUppercut, new object[1] { playerID });
        }

        public void OnKick(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnKick, new object[1] { playerID });
        }

        public void OnThrowOneHandInFists(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnThrowOneHandInFists, new object[1] { playerID });
        }

        public void OnReadyThrowOneHandInFists(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyThrowOneHandInFists, new object[1] { playerID });
        }

        public void OnReadyThrowBothHandInFists(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyThrowBothHandInFists, new object[1] { playerID });
        }

        public void OnReadyHandObliqueCut(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyHandObliqueCut, new object[1] { playerID });
        }

        public void OnWavingOneHand(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnWavingOneHand, new object[1] { playerID });
        }

        public void OnReadyWavingOneHand(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyWavingOneHand, new object[1] { playerID });
        }

        public void OnCombineHandsStraight(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnCombineHandsStraight, new object[1] { playerID });
        }

        public void OnThrowBoulder(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnThrowBoulder, new object[1] { playerID });
        }

        public void OnSlowRun(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlowRun, new object[1] { playerID });
        }

        public void OnFreestyle(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnFreestyle, new object[1] { playerID });
        }

        public void OnKeepRaisingHand(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnKeepRaisingHand, new object[1] { playerID });
        }

        public void OnDeepSquat(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnDeepSquat, new object[1] { playerID });
        }

        public void OnRaiseBothHand(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRaiseBothHand, new object[1] { playerID });
        }

        public void OnArmFlat(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmFlat, new object[1] { playerID });
        }

        public void OnArmFlatIsL(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmFlatIsL, new object[1] { playerID });
        }

        public void OnArmVerticalIsL(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmVerticalIsL, new object[1] { playerID });
        }

        public void OnHandsAway(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsAway, new object[1] { playerID });
        }

        public void OnHandsClose(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsClose, new object[1] { playerID });
        }
        
        public void OnBendBothElbows(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnBendBothElbows, new object[1] { playerID });
        }
        
        public void OnStand(InputValue context)
        {
            bool isActionPressed = context.isPressed;
            if (!isActionPressed) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnStand, new object[1] { playerID });
        }
    }
}

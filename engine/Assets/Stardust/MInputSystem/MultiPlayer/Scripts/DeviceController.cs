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
        private CameraRoamControl _cameraRoamControl;
        private Keyboard keyboard;
        private bool _wKeyStatus = false;
        private bool _sKeyStatus = false;
        private bool _aKeyStatus = false;
        private bool _dKeyStatus = false;
        
        private void Awake()
        {
            keyboard = Keyboard.current;
            if (PlayerBody != null) PlayerBody.SetActive(false);
            if (PlayerCircle != null) PlayerCircle.gameObject.SetActive(false);
            GameObject roam = GameObject.Find("Roam");
            if (roam != null)
            {
                _cameraRoamControl = roam.GetComponent<CameraRoamControl>();
            }
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
            if (!keyboard.wKey.isPressed && _wKeyStatus)
            {
                _wKeyStatus = false;
                _cameraRoamControl?.Stop();
            } else if (!keyboard.sKey.isPressed && _sKeyStatus)
            {
                _sKeyStatus = false;
                _cameraRoamControl?.Stop();
            } else if (!keyboard.aKey.isPressed && _aKeyStatus)
            {
                _aKeyStatus = false;
                _cameraRoamControl?.Stop();
            } else if (!keyboard.dKey.isPressed && _dKeyStatus)
            {
                _dKeyStatus = false;
                _cameraRoamControl?.Stop();
            }
        }

        public bool OnRoam(Vector2 move)
        {
            if (CameraRoamControl.Instance.isRoam)
            {
                _cameraRoamControl?.Start(move);
                return true;
            }

            return false;
        }

        private bool CheckListeningPermissions(string playerId)
        {
            if (DisplayData.configDisplay.interactionPermissionLevel >= 1)
            {
                return PlayerGroup.Instance.GetVirtualPlayerById(playerId) != null;
            }

            return true;
        }

        private bool CheckKeyboardPressed(InputValue context)
        {
            if (!context.isPressed)
            {
                _cameraRoamControl?.Stop();
                return false;
            }
            
            return true;
        }

        public void OnMove(InputValue context)
        {
            Vector2 moveVec = context.Get<Vector2>();
            if (moveVec == Vector2.zero)
            {
                _cameraRoamControl?.Stop();
                return;
            }

            if (!CheckListeningPermissions(_player.id)) return;
            if (OnRoam(moveVec)) return;
            // 手柄模式下区域检测
            if (DisplayData.configDisplay.supportGamepad)
            {
                if (!ROITools.Instance.CheckBoundary(_player.movementInput + moveVec)) return;
            }
            _player.movementInput += moveVec * DisplayData.configDisplay.moveSpeed;
            _player.leftFootInput += moveVec * DisplayData.configDisplay.moveSpeed;
            _player.rightFootInput += moveVec * DisplayData.configDisplay.moveSpeed;
        }

        public void OnRightHandDrawCircle(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRightHandDrawCircle, new object[1] { playerID });
        }

        public void OnLeftHandDrawCircle(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeftHandDrawCircle, new object[1] { playerID });
        }

        public void OnFastRun(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnFastRun, new object[1] { playerID });
        }

        public void OnButterfly(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnButterfly, new object[1] { playerID });
        }

        public void OnApplaud(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnApplaud, new object[1] { playerID });
        }

        public void OnJump(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnJump, new object[1] { playerID });
        }

        public void OnRaiseOnHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            // if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRaiseOnHand, new object[1] { playerID });
        }

        public void OnSlideLeft(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideLeft, new object[1] { playerID });
        }

        public void OnSlideRight(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideRight, new object[1] { playerID });
        }

        public void OnSlideUp(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideUp, new object[1] { playerID });
        }

        public void OnSlideDown(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideDown, new object[1] { playerID });
        }

        public void OnWaving(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnWaving, new object[1] { playerID });
        }

        public void OnArmToForward(InputValue context)
        {
            _wKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(0, 1))) return;
            EventManager.Send(ActionEvent.OnArmToForward, new object[1] { playerID });
        }

        public void OnArmToBack(InputValue context)
        {
            _sKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(0, -1))) return;
            EventManager.Send(ActionEvent.OnArmToBack, new object[1] { playerID });
        }

        public void OnArmToLeft(InputValue context)
        {
            _aKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(-1, 0))) return;
            EventManager.Send(ActionEvent.OnArmToLeft, new object[1] { playerID });
        }

        public void OnArmToRight(InputValue context)
        {
            _dKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;

            if (!CheckListeningPermissions(playerID)) return;
            if (OnRoam(new Vector2(1, 0))) return;
            EventManager.Send(ActionEvent.OnArmToRight, new object[1] { playerID });
        }

        public void OnHandsCross(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            // if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsCross, new object[1] { playerID });
        }

        public void OnPoseA(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseA, new object[1] { playerID });
        }

        public void OnPoseB(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseB, new object[1] { playerID });
        }

        public void OnPoseC(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseC, new object[1] { playerID });
        }

        public void OnPoseD(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseD, new object[1] { playerID });
        }

        public void OnLeanToLeft(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeanToLeft, new object[1] { playerID });
        }

        public void OnLeanToRight(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeanToRight, new object[1] { playerID });
        }

        public void OnSmallSquat(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSmallSquat, new object[1] { playerID });
        }

        // 补全剩下的配置
        public void OnHandBevelCut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandBevelCut, new object[1] { playerID });
        }

        public void OnHandParry(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandParry, new object[1] { playerID });
        }

        public void OnHandStraightCut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandStraightCut, new object[1] { playerID });
        }

        public void OnHandTransversal(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandTransversal, new object[1] { playerID });
        }

        public void OnStraightPunch(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnStraightPunch, new object[1] { playerID });
        }

        public void OnReadyStraightPunch(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyStraightPunch, new object[1] { playerID });
        }

        public void OnUppercut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnUppercut, new object[1] { playerID });
        }

        public void OnKick(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnKick, new object[1] { playerID });
        }

        public void OnThrowOneHandInFists(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnThrowOneHandInFists, new object[1] { playerID });
        }

        public void OnReadyThrowOneHandInFists(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyThrowOneHandInFists, new object[1] { playerID });
        }

        public void OnReadyThrowBothHandInFists(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyThrowBothHandInFists, new object[1] { playerID });
        }

        public void OnReadyHandObliqueCut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyHandObliqueCut, new object[1] { playerID });
        }

        public void OnWavingOneHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnWavingOneHand, new object[1] { playerID });
        }

        public void OnReadyWavingOneHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyWavingOneHand, new object[1] { playerID });
        }

        public void OnCombineHandsStraight(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnCombineHandsStraight, new object[1] { playerID });
        }

        public void OnThrowBoulder(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnThrowBoulder, new object[1] { playerID });
        }

        public void OnSlowRun(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlowRun, new object[1] { playerID });
        }

        public void OnFreestyle(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnFreestyle, new object[1] { playerID });
        }

        public void OnKeepRaisingHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnKeepRaisingHand, new object[1] { playerID });
        }

        public void OnDeepSquat(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnDeepSquat, new object[1] { playerID });
        }

        public void OnRaiseBothHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRaiseBothHand, new object[1] { playerID });
        }

        public void OnArmFlat(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmFlat, new object[1] { playerID });
        }

        public void OnArmFlatIsL(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmFlatIsL, new object[1] { playerID });
        }

        public void OnArmVerticalIsL(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmVerticalIsL, new object[1] { playerID });
        }

        public void OnHandsAway(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsAway, new object[1] { playerID });
        }

        public void OnHandsClose(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsClose, new object[1] { playerID });
        }
        
        public void OnBendBothElbows(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnBendBothElbows, new object[1] { playerID });
        }
        
        public void OnStand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.id;
            if (DevicePlayerManager.Instance.IsGlobalTest) playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnStand, new object[1] { playerID });
        }
    }
}

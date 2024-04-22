using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Stardust.Model;
using Stardust.MultiPlayer.Scripts.MatchPlayer;
using Stardust.Scripts;

namespace Stardust.MultiPlayer.Scripts
{
    /// <summary>
    /// 虚拟设备控制类
    /// </summary>
    public class DeviceController : MonoBehaviour
    {
        private VirtualPlayer _player;
        private MatchRule _matchRule;

        [SerializeField] private int userId;
        private Keyboard _keyboard;
        private bool _wKeyStatus;
        private bool _sKeyStatus;
        private bool _aKeyStatus;
        private bool _dKeyStatus;
        
        private void Awake()
        {
            _keyboard = Keyboard.current;
        }

        private void Start()
        {
            DisplayData.ReadConfig();
            userId = gameObject.GetComponent<PlayerInput>().playerIndex ;
            MDebug.LogFlow("3. 玩家进入 - 3.0 设备信息 userId:" + userId + " name:" + gameObject.GetComponent<PlayerInput>().name);
            _player = new VirtualPlayer(userId.ToString());
            Type type = Type.GetType(DevicePlayerManager.Instance.matchType);
            if (type != null) _matchRule = Activator.CreateInstance(type, new object[] { _player }) as MatchRule;
        }
        
        private void Update()
        {
            _matchRule?.Update();
            if (!_keyboard.wKey.isPressed && _wKeyStatus)
            {
                _wKeyStatus = false;
            } else if (!_keyboard.sKey.isPressed && _sKeyStatus)
            {
                _sKeyStatus = false;
            } else if (!_keyboard.aKey.isPressed && _aKeyStatus)
            {
                _aKeyStatus = false;
            } else if (!_keyboard.dKey.isPressed && _dKeyStatus)
            {
                _dKeyStatus = false;
            }
        }


        private bool CheckListeningPermissions(string playerId)
        {
            if (DisplayData.InteractionPermissionLevel >= 1)
            {
                return PlayerGroup.Instance.GetVirtualPlayerById(playerId) != null;
            }

            return true;
        }

        private bool CheckKeyboardPressed(InputValue context)
        {
            if (!context.isPressed)
            {
                return false;
            }
            
            return true;
        }

        public void OnMove(InputValue context)
        {
            // Vector2 moveVec = context.Get<Vector2>();
            // if (moveVec == Vector2.zero)
            // {
            //     return;
            // }
            //
            // if (!CheckListeningPermissions(_player.ID)) return;
        }

        public void OnRightHandDrawCircle(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID;
            playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRightHandDrawCircle, new object[] { playerID });
        }

        public void OnLeftHandDrawCircle(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID;
            playerID = DevicePlayerManager.Instance.BeControlledUserID;
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeftHandDrawCircle, new object[] { playerID });
        }

        public void OnFastRun(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnFastRun, new object[] { playerID });
        }

        public void OnButterfly(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnButterfly, new object[] { playerID });
        }

        public void OnApplaud(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnApplaud, new object[] { playerID });
        }

        public void OnJump(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnJump, new object[] { playerID });
        }

        public void OnRaiseOnHand(InputValue context)
        {
            MDebug.LogFlow("3. 玩家进入 - 3.0 举右手");
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            // if (!CheckListeningPermissions(playerID)) return;
            MDebug.LogFlow("3. 玩家进入 - 3.0.2 举右手玩家ID：" + playerID);
            EventManager.Send(ActionEvent.OnRaiseOnHand, new object[] { playerID });
        }

        public void OnSlideLeft(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideLeft, new object[] { playerID });
        }

        public void OnSlideRight(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideRight, new object[] { playerID });
        }

        public void OnSlideUp(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideUp, new object[] { playerID });
        }

        public void OnSlideDown(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlideDown, new object[] { playerID });
        }

        public void OnWaving(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnWaving, new object[] { playerID });
        }

        public void OnArmToForward(InputValue context)
        {
            _wKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             

            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmToForward, new object[] { playerID });
        }

        public void OnArmToBack(InputValue context)
        {
            _sKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             

            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmToBack, new object[] { playerID });
        }

        public void OnArmToLeft(InputValue context)
        {
            _aKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             

            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmToLeft, new object[] { playerID });
        }

        public void OnArmToRight(InputValue context)
        {
            _dKeyStatus = true;
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             

            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmToRight, new object[] { playerID });
        }

        public void OnHandsCross(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            // if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsCross, new object[] { playerID });
        }

        public void OnPoseA(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseA, new object[] { playerID });
        }

        public void OnPoseB(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseB, new object[] { playerID });
        }

        public void OnPoseC(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseC, new object[] { playerID });
        }

        public void OnPoseD(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnPoseD, new object[] { playerID });
        }

        public void OnLeanToLeft(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeanToLeft, new object[] { playerID });
        }

        public void OnLeanToRight(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnLeanToRight, new object[] { playerID });
        }

        public void OnSmallSquat(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSmallSquat, new object[] { playerID });
        }

        // 补全剩下的配置
        public void OnHandBevelCut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandBevelCut, new object[] { playerID });
        }

        public void OnHandParry(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandParry, new object[] { playerID });
        }

        public void OnHandStraightCut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandStraightCut, new object[] { playerID });
        }

        public void OnHandTransversal(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandTransversal, new object[] { playerID });
        }

        public void OnStraightPunch(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnStraightPunch, new object[] { playerID });
        }

        public void OnReadyStraightPunch(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyStraightPunch, new object[] { playerID });
        }

        public void OnUppercut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnUppercut, new object[] { playerID });
        }

        public void OnKick(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnKick, new object[] { playerID });
        }

        public void OnThrowOneHandInFists(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnThrowOneHandInFists, new object[] { playerID });
        }

        public void OnReadyThrowOneHandInFists(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyThrowOneHandInFists, new object[] { playerID });
        }

        public void OnReadyThrowBothHandInFists(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyThrowBothHandInFists, new object[] { playerID });
        }

        public void OnReadyHandObliqueCut(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyHandObliqueCut, new object[] { playerID });
        }

        public void OnWavingOneHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnWavingOneHand, new object[] { playerID });
        }

        public void OnReadyWavingOneHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnReadyWavingOneHand, new object[] { playerID });
        }

        public void OnCombineHandsStraight(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnCombineHandsStraight, new object[] { playerID });
        }

        public void OnThrowBoulder(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnThrowBoulder, new object[] { playerID });
        }

        public void OnSlowRun(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnSlowRun, new object[] { playerID });
        }

        public void OnFreestyle(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnFreestyle, new object[] { playerID });
        }

        public void OnKeepRaisingHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnKeepRaisingHand, new object[] { playerID });
        }

        public void OnDeepSquat(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnDeepSquat, new object[] { playerID });
        }

        public void OnRaiseBothHand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnRaiseBothHand, new object[] { playerID });
        }

        public void OnArmFlat(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmFlat, new object[] { playerID });
        }

        public void OnArmFlatIsL(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmFlatIsL, new object[] { playerID });
        }

        public void OnArmVerticalIsL(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnArmVerticalIsL, new object[] { playerID });
        }

        public void OnHandsAway(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsAway, new object[] { playerID });
        }

        public void OnHandsClose(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnHandsClose, new object[] { playerID });
        }
        
        public void OnBendBothElbows(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnBendBothElbows, new object[] { playerID });
        }
        
        public void OnStand(InputValue context)
        {
            if (!CheckKeyboardPressed(context)) return;
            string playerID = _player.ID;
             
            if (!CheckListeningPermissions(playerID)) return;
            EventManager.Send(ActionEvent.OnStand, new object[] { playerID });
        }
    }
}

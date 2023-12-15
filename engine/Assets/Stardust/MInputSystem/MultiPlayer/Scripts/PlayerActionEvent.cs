using UnityEngine;
using Moat.Model;

namespace Moat
{
    /// <summary>
    /// 事件调试类
    /// </summary>
    public class PlayerActionEvent : MonoBehaviour
    {
        void Start()
        {
            EventManager.RegisterListener(ActionEvent.OnRightHandDrawCircle, OnRightHandDrawCircle);
            EventManager.RegisterListener(ActionEvent.OnLeftHandDrawCircle, OnLeftHandDrawCircle);
            EventManager.RegisterListener(ActionEvent.OnHandBevelCut, OnHandBevelCut);
            EventManager.RegisterListener(ActionEvent.OnHandParry, OnHandParry);
            EventManager.RegisterListener(ActionEvent.OnHandStraightCut, OnHandStraightCut);
            EventManager.RegisterListener(ActionEvent.OnHandTransversal, OnHandTransversal);
            EventManager.RegisterListener(ActionEvent.OnStraightPunch, OnStraightPunch);
            EventManager.RegisterListener(ActionEvent.OnReadyStraightPunch, OnReadyStraightPunch);
            EventManager.RegisterListener(ActionEvent.OnUppercut, OnUppercut);
            EventManager.RegisterListener(ActionEvent.OnKick, OnKick);
            EventManager.RegisterListener(ActionEvent.OnThrowOneHandInFists, OnThrowOneHandInFists);
            EventManager.RegisterListener(ActionEvent.OnReadyThrowOneHandInFists, OnReadyThrowOneHandInFists);
            EventManager.RegisterListener(ActionEvent.OnReadyThrowBothHandInFists, OnReadyThrowBothHandInFists);
            EventManager.RegisterListener(ActionEvent.OnReadyHandObliqueCut, OnReadyHandObliqueCut);
            EventManager.RegisterListener(ActionEvent.OnWavingOneHand, OnWavingOneHand);
            EventManager.RegisterListener(ActionEvent.OnReadyWavingOneHand, OnReadyWavingOneHand);
            EventManager.RegisterListener(ActionEvent.OnCombineHandsStraight, OnCombineHandsStraight);
            EventManager.RegisterListener(ActionEvent.OnThrowBoulder, OnThrowBoulder);
            EventManager.RegisterListener(ActionEvent.OnSlowRun, OnSlowRun);
            EventManager.RegisterListener(ActionEvent.OnFastRun, OnFastRun);
            EventManager.RegisterListener(ActionEvent.OnButterfly, OnButterfly);
            EventManager.RegisterListener(ActionEvent.OnFreestyle, OnFreestyle);
            EventManager.RegisterListener(ActionEvent.OnKeepRaisingHand, OnKeepRaisingHand);
            EventManager.RegisterListener(ActionEvent.OnApplaud, OnApplaud);
            EventManager.RegisterListener(ActionEvent.OnJump, OnJump);
            EventManager.RegisterListener(ActionEvent.OnDeepSquat, OnDeepSquat);
            EventManager.RegisterListener(ActionEvent.OnRaiseOnHand, OnRaiseOnHand);
            EventManager.RegisterListener(ActionEvent.OnRaiseBothHand, OnRaiseBothHand);
            EventManager.RegisterListener(ActionEvent.OnArmFlat, OnArmFlat);
            EventManager.RegisterListener(ActionEvent.OnArmFlatIsL, OnArmFlatIsL);
            EventManager.RegisterListener(ActionEvent.OnArmVerticalIsL, OnArmVerticalIsL);
            EventManager.RegisterListener(ActionEvent.OnSlideLeft, OnSlideLeft);
            EventManager.RegisterListener(ActionEvent.OnSlideRight, OnSlideRight);
            EventManager.RegisterListener(ActionEvent.OnSlideUp, OnSlideUp);
            EventManager.RegisterListener(ActionEvent.OnSlideDown, OnSlideDown);
            EventManager.RegisterListener(ActionEvent.OnHandsAway, OnHandsAway);
            EventManager.RegisterListener(ActionEvent.OnHandsClose, OnHandsClose);
            EventManager.RegisterListener(ActionEvent.OnWaving, OnWaving);
            EventManager.RegisterListener(ActionEvent.OnArmToForward, OnArmToForward);
            EventManager.RegisterListener(ActionEvent.OnArmToBack, OnArmToBack);
            EventManager.RegisterListener(ActionEvent.OnArmToLeft, OnArmToLeft);
            EventManager.RegisterListener(ActionEvent.OnArmToRight, OnArmToRight);
            EventManager.RegisterListener(ActionEvent.OnBendBothElbows, OnBendBothElbows);
            EventManager.RegisterListener(ActionEvent.OnHandsCross, OnHandsCross);
            EventManager.RegisterListener(ActionEvent.OnPoseA, OnPoseA);
            EventManager.RegisterListener(ActionEvent.OnPoseB, OnPoseB);
            EventManager.RegisterListener(ActionEvent.OnPoseC, OnPoseC);
            EventManager.RegisterListener(ActionEvent.OnPoseD, OnPoseD);
            EventManager.RegisterListener(ActionEvent.OnLeanToLeft, OnLeanToLeft);
            EventManager.RegisterListener(ActionEvent.OnLeanToRight, OnLeanToRight);
            EventManager.RegisterListener(ActionEvent.OnStand, OnStand);
            EventManager.RegisterListener(ActionEvent.OnSmallSquat, OnSmallSquat);
        }

        private void OnHandsCross(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandsCross - 双手交叉");
            if (!DisplayData.configDisplay.allowClose) return;
            AppClose.Instance.OpenThrottle();
        }

       private void OnRightHandDrawCircle(EventCallBack evt)
        {
            MDebug.Log("交互 - OnRightHandDrawCircle - 右手画圈");
        }

        private void OnLeftHandDrawCircle(EventCallBack evt)
        {
            MDebug.Log("交互 - OnLeftHandDrawCircle - 左手画圈");
        }

        private void OnHandBevelCut(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandBevelCut - 手斜切");
        }

        private void OnHandParry(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandParry - 手挡开");
        }

        private void OnHandStraightCut(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandStraightCut - 手直切");
        }

        private void OnHandTransversal(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandTransversal - 手横切");
        }

        private void OnStraightPunch(EventCallBack evt)
        {
            MDebug.Log("交互 - OnStraightPunch - 直拳");
        }

        private void OnReadyStraightPunch(EventCallBack evt)
        {
            MDebug.Log("交互 - OnReadyStraightPunch - 蓄力直拳");
        }

        private void OnUppercut(EventCallBack evt)
        {
            MDebug.Log("交互 - OnUppercut - 上勾拳");
        }

        private void OnKick(EventCallBack evt)
        {
            MDebug.Log("交互 - OnKick - 踢腿");
        }

        private void OnThrowOneHandInFists(EventCallBack evt)
        {
            MDebug.Log("交互 - OnThrowOneHandInFists - 单手握拳投掷");
        }

        private void OnReadyThrowOneHandInFists(EventCallBack evt)
        {
            MDebug.Log("交互 - OnReadyThrowOneHandInFists - 蓄力单手握拳投掷");
        }

        private void OnReadyThrowBothHandInFists(EventCallBack evt)
        {
            MDebug.Log("交互 - OnReadyThrowBothHandInFists - 蓄力双手握拳投掷");
        }

        private void OnReadyHandObliqueCut(EventCallBack evt)
        {
            MDebug.Log("交互 - OnReadyHandObliqueCut - 蓄力手斜切");
        }

        private void OnWavingOneHand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnWavingOneHand - 单手挥舞");
        }

        private void OnReadyWavingOneHand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnReadyWavingOneHand - 蓄力单手挥舞");
        }

        private void OnCombineHandsStraight(EventCallBack evt)
        {
            MDebug.Log("交互 - OnCombineHandsStraight - 双手伸直合并");
        }

        private void OnThrowBoulder(EventCallBack evt)
        {
            MDebug.Log("交互 - OnThrowBoulder - 举手投掷巨物");
        }

        private void OnSlowRun(EventCallBack evt)
        {
            MDebug.Log("交互 - OnSlowRun - 慢跑");
        }

        private void OnFastRun(EventCallBack evt)
        {
            MDebug.Log("交互 - OnFastRun - 快跑");
        }

        private void OnButterfly(EventCallBack evt)
        {
            MDebug.Log("交互 - OnButterfly - 蝶泳");
        }

        private void OnFreestyle(EventCallBack evt)
        {
            MDebug.Log("交互 - OnFreestyle - 自由泳");
        }

        private void OnKeepRaisingHand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnKeepRaisingHand - 持续举手");
        }

        private void OnApplaud(EventCallBack evt)
        {
            MDebug.Log("交互 - OnApplaud - 拍掌");
        }

        private void OnJump(EventCallBack evt)
        {
            MDebug.Log("交互 - OnJump - 起跳");
        }

        private void OnDeepSquat(EventCallBack evt)
        {
            MDebug.Log("交互 - OnDeepSquat - 下蹲");
        }

        private void OnRaiseOnHand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnRaiseOnHand - 举单手");
        }

        private void OnRaiseBothHand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnRaiseBothHand - 举双手");
        }

        private void OnArmFlat(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmFlat - 手臂平展");
        }

        private void OnArmFlatIsL(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmFlatIsL - 手臂平展为L");
        }

        private void OnArmVerticalIsL(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmVerticalIsL - 手臂垂直为L");
        }

        private void OnSlideLeft(EventCallBack evt)
        {
            MDebug.Log("交互 - OnSlideLeft - 左滑");
        }

        private void OnSlideRight(EventCallBack evt)
        {
            MDebug.Log("交互 - OnSlideRight - 右滑");
        }

        private void OnSlideUp(EventCallBack evt)
        {
            MDebug.Log("交互 - OnSlideUp - 上滑");
        }

        private void OnSlideDown(EventCallBack evt)
        {
            MDebug.Log("交互 - OnSlideDown - 下滑");
        }

        private void OnHandsAway(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandsAway - 双手远离");
        }

        private void OnHandsClose(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandsClose - 双手靠近");
        }

        private void OnWaving(EventCallBack evt)
        {
            MDebug.Log("交互 - OnWaving - 挥手打招呼");
        }

        private void OnArmToForward(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmToForward - 手臂向前");
        }

        private void OnArmToBack(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmToBack - 手臂向后");
        }

        private void OnArmToLeft(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmToLeft - 手臂向左");
        }

        private void OnArmToRight(EventCallBack evt)
        {
            MDebug.Log("交互 - OnArmToRight - 手臂向右");
        }

        private void OnBendBothElbows(EventCallBack evt)
        {
            MDebug.Log("交互 - OnBendBothElbows - 弯曲双肘");
        }

        private void OnPoseA(EventCallBack evt)
        {
            MDebug.Log("交互 - OnPoseA - 姿态A");
        }

        private void OnPoseB(EventCallBack evt)
        {
            MDebug.Log("交互 - OnPoseB - 姿态B");
        }

        private void OnPoseC(EventCallBack evt)
        {
            MDebug.Log("交互 - OnPoseC - 姿态C");
        }

        private void OnPoseD(EventCallBack evt)
        {
            MDebug.Log("交互 - OnPoseD - 姿态D");
        }

        private void OnLeanToLeft(EventCallBack evt)
        {
            MDebug.Log("交互 - OnLeanToLeft - 身体左倾");
        }

        private void OnLeanToRight(EventCallBack evt)
        {
            MDebug.Log("交互 - OnLeanToRight - 身体右倾");
        }

        private void OnStand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnStand - 站立");
        }

        private void OnSmallSquat(EventCallBack evt)
        {
            MDebug.Log("交互 - OnSmallSquat - 浅蹲");
        } 
        
        void OnDestroy()
        {
            EventManager.RemoveListener(ActionEvent.OnRightHandDrawCircle, OnRightHandDrawCircle);
            EventManager.RemoveListener(ActionEvent.OnLeftHandDrawCircle, OnLeftHandDrawCircle);
            EventManager.RemoveListener(ActionEvent.OnHandBevelCut, OnHandBevelCut);
            EventManager.RemoveListener(ActionEvent.OnHandParry, OnHandParry);
            EventManager.RemoveListener(ActionEvent.OnHandStraightCut, OnHandStraightCut);
            EventManager.RemoveListener(ActionEvent.OnHandTransversal, OnHandTransversal);
            EventManager.RemoveListener(ActionEvent.OnStraightPunch, OnStraightPunch);
            EventManager.RemoveListener(ActionEvent.OnReadyStraightPunch, OnReadyStraightPunch);
            EventManager.RemoveListener(ActionEvent.OnUppercut, OnUppercut);
            EventManager.RemoveListener(ActionEvent.OnKick, OnKick);
            EventManager.RemoveListener(ActionEvent.OnThrowOneHandInFists, OnThrowOneHandInFists);
            EventManager.RemoveListener(ActionEvent.OnReadyThrowOneHandInFists, OnReadyThrowOneHandInFists);
            EventManager.RemoveListener(ActionEvent.OnReadyThrowBothHandInFists, OnReadyThrowBothHandInFists);
            EventManager.RemoveListener(ActionEvent.OnReadyHandObliqueCut, OnReadyHandObliqueCut);
            EventManager.RemoveListener(ActionEvent.OnWavingOneHand, OnWavingOneHand);
            EventManager.RemoveListener(ActionEvent.OnReadyWavingOneHand, OnReadyWavingOneHand);
            EventManager.RemoveListener(ActionEvent.OnCombineHandsStraight, OnCombineHandsStraight);
            EventManager.RemoveListener(ActionEvent.OnThrowBoulder, OnThrowBoulder);
            EventManager.RemoveListener(ActionEvent.OnSlowRun, OnSlowRun);
            EventManager.RemoveListener(ActionEvent.OnFastRun, OnFastRun);
            EventManager.RemoveListener(ActionEvent.OnButterfly, OnButterfly);
            EventManager.RemoveListener(ActionEvent.OnFreestyle, OnFreestyle);
            EventManager.RemoveListener(ActionEvent.OnKeepRaisingHand, OnKeepRaisingHand);
            EventManager.RemoveListener(ActionEvent.OnApplaud, OnApplaud);
            EventManager.RemoveListener(ActionEvent.OnJump, OnJump);
            EventManager.RemoveListener(ActionEvent.OnDeepSquat, OnDeepSquat);
            EventManager.RemoveListener(ActionEvent.OnRaiseOnHand, OnRaiseOnHand);
            EventManager.RemoveListener(ActionEvent.OnRaiseBothHand, OnRaiseBothHand);
            EventManager.RemoveListener(ActionEvent.OnArmFlat, OnArmFlat);
            EventManager.RemoveListener(ActionEvent.OnArmFlatIsL, OnArmFlatIsL);
            EventManager.RemoveListener(ActionEvent.OnArmVerticalIsL, OnArmVerticalIsL);
            EventManager.RemoveListener(ActionEvent.OnSlideLeft, OnSlideLeft);
            EventManager.RemoveListener(ActionEvent.OnSlideRight, OnSlideRight);
            EventManager.RemoveListener(ActionEvent.OnSlideUp, OnSlideUp);
            EventManager.RemoveListener(ActionEvent.OnSlideDown, OnSlideDown);
            EventManager.RemoveListener(ActionEvent.OnHandsAway, OnHandsAway);
            EventManager.RemoveListener(ActionEvent.OnHandsClose, OnHandsClose);
            EventManager.RemoveListener(ActionEvent.OnWaving, OnWaving);
            EventManager.RemoveListener(ActionEvent.OnArmToForward, OnArmToForward);
            EventManager.RemoveListener(ActionEvent.OnArmToBack, OnArmToBack);
            EventManager.RemoveListener(ActionEvent.OnArmToLeft, OnArmToLeft);
            EventManager.RemoveListener(ActionEvent.OnArmToRight, OnArmToRight);
            EventManager.RemoveListener(ActionEvent.OnBendBothElbows, OnBendBothElbows);
            EventManager.RemoveListener(ActionEvent.OnHandsCross, OnHandsCross);
            EventManager.RemoveListener(ActionEvent.OnPoseA, OnPoseA);
            EventManager.RemoveListener(ActionEvent.OnPoseB, OnPoseB);
            EventManager.RemoveListener(ActionEvent.OnPoseC, OnPoseC);
            EventManager.RemoveListener(ActionEvent.OnPoseD, OnPoseD);
            EventManager.RemoveListener(ActionEvent.OnLeanToLeft, OnLeanToLeft);
            EventManager.RemoveListener(ActionEvent.OnLeanToRight, OnLeanToRight);
            EventManager.RemoveListener(ActionEvent.OnStand, OnStand);
            EventManager.RemoveListener(ActionEvent.OnSmallSquat, OnSmallSquat);
        }
    }
}
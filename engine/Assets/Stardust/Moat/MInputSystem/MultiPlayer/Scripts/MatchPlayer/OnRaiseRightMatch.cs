using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodySource;
using Moat.Model;
using DGXR;

namespace Moat
{
    /// <summary>
    /// 右手匹配规则-不处理调试模式逻辑
    /// </summary>
    public class OnRaiseRightMatch : IMatchRule
    {
        private readonly VirtualPlayer _player;
        public OnRaiseRightMatch(VirtualPlayer player)
        {
            _player = player;
            EventManager.RegisterListener(ActionEvent.OnRaiseOnHand,OnRaiseOnHand);
        }

        private async void OnRaiseOnHand(EventCallBack evt)
        {
            MDebug.Log("OnRaiseOnHand:"+(string)evt.Params[0]+"--"+_player.id + "  " + DevicePlayerManager.Instance.IsGlobalTest + "-" + !DisplayData.configDisplay.supportGamepad);
            if(DevicePlayerManager.Instance.IsGlobalTest && !DisplayData.configDisplay.supportGamepad) return;
            if ((string)evt.Params[0] == _player.id)
            {
                PlayerGroup.Instance.AddPlayer(_player);
                
                if (DisplayData.configDisplay.playerCount > 1) return;
                if (DisplayData.configDisplay.forcedSubstitutionsInSinglePlayer)
                {
                    MDebug.LogTest("PlayerGroup.Instance1: " + PlayerGroup.Instance.players.Count);
                    PlayerGroup.Instance.RemovePlayerByIndex(0);
                    MDebug.LogTest("PlayerGroup.Instance2: " + PlayerGroup.Instance.players.Count);
                    await Task.Delay(100);
                    PlayerGroup.Instance.AddPlayer(_player);
                    MDebug.LogTest("PlayerGroup.Instance3: " + PlayerGroup.Instance.players.Count);
                }

                if (DisplayData.configDisplay.allowFollowingInSinglePlayer)
                {
                    XRDGBodySource.Instance.SetCavePersonId(_player.id);
                    XRWorldManager.instance.LockAll = false;
                }
                else
                {
                    XRWorldManager.instance.LockAll = true;
                }
            }
        }

        public override void Update()
        {
            if(DevicePlayerManager.Instance.IsGlobalTest || DisplayData.configDisplay.supportGamepad) return;
            //todo  如果是外接设备
            if (int.Parse(_player.id) > 10) return;
            Dictionary<string, PersonBody> personBodyInfo = DevicePlayerManager.Instance.PersonBodyInfo;
            //MDebug.Log("personBodyInfo.Count::"+personBodyInfo.Count);
            if (!personBodyInfo.Keys.Contains(this._player.id))
            {
                PlayerGroup.Instance.RemovePlayer(_player);
            }
            else
            {
                personBodyInfo.TryGetValue(_player.id, out PersonBody body);
                _player.movementInput = body.movementInput;
                _player.leftFootInput = body.leftFootInput;
                _player.rightFootInput = body.rightFootInput;
            }
        }
    }

}

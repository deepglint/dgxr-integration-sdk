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
            if ((string)evt.Params[0] == _player.id || evt.Params[0] == null)
            {
                MDebug.LogFlow("3. 玩家进入 - 3.0.3 举右手匹配 " + (string)evt.Params[0]+"--"+_player.id);
                if (DevicePlayerManager.Instance.IsGlobalTest && !DisplayData.supportGamepad)
                {
                    MDebug.LogFlow("3. 玩家进入 - 3.0.5 不连接ws && 不支持手柄");
                    return;
                }
                MDebug.LogFlow("3. 玩家进入 - 3.1 玩家强进权限" + DisplayData.forcedSubstitutionsInSinglePlayer);
                MDebug.LogFlow("3. 玩家进入 - 3.2 玩家匹配成功" + evt.Params[0]);
                if (DisplayData.forcedSubstitutionsInSinglePlayer && DisplayData.configDisplay.playerCount == 1)
                {
                    PlayerGroup.Instance.ReplacePlayer(_player, 0);
                    MDebug.LogFlow("4. 视角跟随 - 1.0 权限 " + DisplayData.allowFollowingInSinglePlayer);
                    if (DisplayData.allowFollowingInSinglePlayer)
                    {
                        XRWorldManager.instance.LockAll = false;
                        XRDGBodySource.Instance.SetCavePersonId(_player.id);
                    }
                    else
                    {
                        XRWorldManager.instance.LockAll = true;
                    } 
                }
                else
                {
                    PlayerGroup.Instance.AddPlayer(_player); 
                }
            }
            else
            {
                MDebug.Log("3. 玩家进入 - 3.0.3 举右手不匹配 " + (string)evt.Params[0]+"--"+_player.id);
            }
        }

        public override void Update()
        {
            if(DevicePlayerManager.Instance.IsGlobalTest || DisplayData.supportGamepad) return;
            // todo  如果是外接设备
            if (int.Parse(_player.id) > 10) return;
            Dictionary<string, PersonBody> personBodyInfo = DevicePlayerManager.Instance.PersonBodyInfo;
            // MDebug.Log("personBodyInfo.Count::"+personBodyInfo.Count);
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

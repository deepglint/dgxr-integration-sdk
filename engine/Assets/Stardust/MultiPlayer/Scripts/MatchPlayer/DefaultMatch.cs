using System;
using System.Collections.Generic;
using System.Linq;
using Stardust.Model;
using Stardust.Scripts;
using Stardust.URP;

namespace Stardust.MultiPlayer.Scripts.MatchPlayer 
{
    /// <summary>
    /// 右手匹配规则-不处理调试模式逻辑
    /// </summary>
    public class DefaultMatch : MatchRule 
    {
        private readonly VirtualPlayer _player;
        public DefaultMatch(VirtualPlayer player)
        {
            _player = player;
            EventManager.RegisterListener(ActionEvent.OnRaiseOnHand,OnRaiseOnHand);
        }

        private void OnRaiseOnHand(EventCallBack evt)
        {
            if ((string)evt.Params[0] == _player.ID || evt.Params[0] == null)
            {
                MDebug.LogFlow("3. 玩家进入 - 3.0.3 举右手匹配 " + (string)evt.Params[0]+"--"+_player.ID);
                MDebug.LogFlow("3. 玩家进入 - 3.1 玩家强进权限" + DisplayData.ForcedSubstitutionsInSinglePlayer);
                MDebug.LogFlow("3. 玩家进入 - 3.2 玩家匹配成功" + evt.Params[0]);
                if (DisplayData.ForcedSubstitutionsInSinglePlayer && DisplayData.ConfigDisplay.PlayerCount == 1)
                {
                    PlayerGroup.Instance.ReplacePlayer(_player, 0);
                    MDebug.LogFlow("4. 视角跟随 - 1.0 权限 " + DisplayData.AllowFollowingInSinglePlayer);
                    if (DisplayData.AllowFollowingInSinglePlayer)
                    {
                        XRWorldManagerUrp.Instance.lockAll = false;
                        XrdgBodySource.Instance.SetCavePersonId(_player.ID); 
                    }
                    else
                    {
                        XRWorldManagerUrp.Instance.lockAll = true;
                    } 
                }
                else
                {
                    PlayerGroup.Instance.AddPlayer(_player); 
                }
            }
            else
            {
                MDebug.Log("3. 玩家进入 - 3.0.3 举右手不匹配 " + (string)evt.Params[0]+"--"+_player.ID);
            }
        }

        public override void Update()
        {
            // todo  如果是外接设备
            if (int.Parse(_player.ID) > 10) return;
            Dictionary<string, PersonBody> personBodyInfo = DevicePlayerManager.Instance.PersonBodyInfo;
            // MDebug.Log("personBodyInfo.Count::"+personBodyInfo.Count);
            if (!personBodyInfo.Keys.Contains(this._player.ID))
            {
                PlayerGroup.Instance.RemovePlayer(_player);
            }
            else
            {
                personBodyInfo.TryGetValue(_player.ID, out PersonBody body);
                if (body != null)
                {
                    _player.MovementInput = body.MovementInput;
                    _player.LeftFootInput = body.LeftFootInput;
                    _player.RightFootInput = body.RightFootInput;
                }
            }
        }
    }

}

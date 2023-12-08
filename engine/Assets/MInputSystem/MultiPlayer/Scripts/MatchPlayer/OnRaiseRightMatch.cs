using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Moat;

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

        private void OnRaiseOnHand(EventCallBack evt)
        {
            MDebug.Log("OnRaiseOnHand:"+(string)evt.Params[0]+"--"+_player.id);
            if (DevicePlayerManager.Instance.IsGlobalTest) return;
            if ((string)evt.Params[0] == _player.id)
            {
                PlayerGroup.Instance.AddPlayer(_player);
            }
        }

        public override void Update()
        {
            if (DevicePlayerManager.Instance.IsGlobalTest) return;
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

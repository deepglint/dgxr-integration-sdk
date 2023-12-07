using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Moat;

namespace Moat
{
    /// <summary>
    /// 准备了的虚拟玩家
    /// </summary>
    public class PlayerGroup: MSingleton<PlayerGroup>
    {
        public List<VirtualPlayer> players = new List<VirtualPlayer>();
        public int maxCount = 1;

        public void RemovePlayer(VirtualPlayer player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
                EventManager.Send(MoatGameEvent.PlayerRemove, new object[1]{ player.id });
            }
        }
        
        public void RemovePlayerById(string playerID)
        {
            MDebug.Log("RemovePlayerById::"+playerID + "   Lenght:"+ players.Count);
            foreach (var player in players)
            {
                if (player.id == playerID)
                {
                    players.Remove(player);
                    EventManager.Send(MoatGameEvent.PlayerRemove, new object[1]{ player.id });
                    break;
                }
            }
            MDebug.Log("RemovePlayerById Lenght:"+ players.Count);
        }

        public void AddPlayer(VirtualPlayer player)
        {
            MDebug.Log("AddPlayer::"+player.id + "   Lenght:"+ players.Count);
            if (players.Count < maxCount)
            {
                if (!players.Contains(player))
                {
                    players.Add(player);
                    EventManager.Send(MoatGameEvent.PlayerAdd, new object[1]{ player.id });
                }
            }
            MDebug.Log("AddPlayer Lenght:"+ players.Count);
        }

        public bool IsMaxPlayerCount()
        {
            return players.Count >= maxCount;
        }

        public VirtualPlayer GetVirtualPlayerById(string id)
        {
            foreach (var vPlayer in players)
            {
                if (vPlayer.id == id)
                {
                    return vPlayer;
                }
            }
            return null;
        }
        
        
        
        //=================================================================================
        
        public int count = 1;

        public bool IsAllReady()
        {
            int flagCount = 0;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].isReady)
                {
                    flagCount += 1;
                }
            }

            return flagCount == maxCount;
        }

        public void SetNumberPlayer(int _count)
        {
            count = _count;
            players = new List<VirtualPlayer>();
        }

        public bool IsEnterRoi(int moveArea)
        {
            bool singleIsEnterRoi = maxCount == 1 && moveArea > 0;
            if (singleIsEnterRoi)
            {
                return singleIsEnterRoi;
            }

            bool multiIsEnterRoi = maxCount > 1 && (0 < moveArea & moveArea < 5);
            if (multiIsEnterRoi)
            {
                return multiIsEnterRoi;
            }

            return false;
        }

    }
}

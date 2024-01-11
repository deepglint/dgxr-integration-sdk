using System.Collections.Generic;

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

        public void RemovePlayerByIndex(int index)
        {
            if (players.Count > index && players[index] != null)
            {
                VirtualPlayer player = players[index];
                players.Remove(player);
                EventManager.Send(MoatGameEvent.PlayerRemove, new object[1]{ player.id });
            }
        }

        public void RemovePlayerById(string playerID)
        {
            MDebug.LogTest("RemovePlayerById::"+playerID + "   Lenght:"+ players.Count);
            foreach (var player in players)
            {
                if (player.id == playerID)
                {
                    players.Remove(player);
                    
                    EventManager.Send(MoatGameEvent.PlayerRemove, new object[1]{ player.id });
                    break;
                }
            }
            MDebug.LogTest("RemovePlayerById Lenght:"+ players.Count);
        }

        public void AddPlayer(VirtualPlayer player)
        {
            if (players.Contains(player)) return;
            if (players.Count < maxCount)
            {
                players.Add(player);
                MDebug.LogFlow("3. 玩家进入 - 3.1.1 玩家【" + player.id + "】进入成功");
                EventManager.Send(MoatGameEvent.PlayerAdd, new object[1]{ player.id }); 
            }
            else
            {
                MDebug.LogFlow("3. 玩家进入 - 3.1.2 玩家【" + player.id + "】进入失败"); 
            }
            MDebug.LogFlow("3. 玩家进入 - 3.3 玩家Groups人数: "+ players.Count);
        }
        
        public void ReplacePlayer(VirtualPlayer player, int index)
        {
            if (players.Contains(player)) return;
            if (players.Count < maxCount)
            {
                players.Add(player);
                MDebug.LogFlow("3. 玩家进入 - 3.1.3 替换玩家【" + player.id + "】加入成功");
                EventManager.Send(MoatGameEvent.PlayerAdd, new object[1]{ player.id }); 
            }
            else
            {
                VirtualPlayer prePlayer = players[index];
                RemovePlayerById(prePlayer.id);
                players.Add(player);
                MDebug.LogFlow("3. 玩家进入 - 3.1.3 替换玩家【" + player.id + "】替换成功");
                EventManager.Send(MoatGameEvent.PlayerAdd, new object[1]{ player.id });
            }
            MDebug.LogFlow("3. 玩家进入 - 3.3 玩家Groups人数: "+ players.Count);
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

using System.Collections.Generic;
using Stardust.Scripts;

namespace Stardust.MultiPlayer.Scripts
{
    /// <summary>
    /// 准备了的虚拟玩家
    /// </summary>
    public class PlayerGroup: MSingleton<PlayerGroup>
    {
        public List<VirtualPlayer> Players = new List<VirtualPlayer>();
        public int MaxCount = 1;

        public void RemovePlayer(VirtualPlayer player)
        {
            if (Players.Contains(player))
            {
                Players.Remove(player);
                EventManager.Send(MoatGameEvent.PlayerRemove, new object[]{ player.ID });
            }
        }

        public void RemovePlayerByIndex(int index)
        {
            if (Players.Count > index && Players[index] != null)
            {
                VirtualPlayer player = Players[index];
                Players.Remove(player);
                EventManager.Send(MoatGameEvent.PlayerRemove, new object[]{ player.ID });
            }
        }

        public void RemovePlayerById(string playerID)
        {
            MDebug.LogTest("RemovePlayerById::"+playerID + "   Lenght:"+ Players.Count);
            foreach (var player in Players)
            {
                if (player.ID == playerID)
                {
                    Players.Remove(player);
                    
                    EventManager.Send(MoatGameEvent.PlayerRemove, new object[]{ player.ID });
                    break;
                }
            }
            MDebug.LogTest("RemovePlayerById Lenght:"+ Players.Count);
        }

        public void AddPlayer(VirtualPlayer player)
        {
            if (Players.Contains(player)) return;
            if (Players.Count < MaxCount)
            {
                Players.Add(player);
                MDebug.LogFlow("3. 玩家进入 - 3.1.1 玩家【" + player.ID + "】进入成功");
                EventManager.Send(MoatGameEvent.PlayerAdd, new object[]{ player.ID }); 
            }
            else
            {
                MDebug.LogFlow("3. 玩家进入 - 3.1.2 玩家【" + player.ID + "】进入失败"); 
            }
            MDebug.LogFlow("3. 玩家进入 - 3.3 玩家Groups人数: "+ Players.Count);
        }
        
        public void ReplacePlayer(VirtualPlayer player, int index)
        {
            if (Players.Contains(player)) return;
            if (Players.Count < MaxCount)
            {
                Players.Add(player);
                MDebug.LogFlow("3. 玩家进入 - 3.1.3 替换玩家【" + player.ID + "】加入成功");
                EventManager.Send(MoatGameEvent.PlayerAdd, new object[]{ player.ID }); 
            }
            else
            {
                VirtualPlayer prePlayer = Players[index];
                RemovePlayerById(prePlayer.ID);
                Players.Add(player);
                MDebug.LogFlow("3. 玩家进入 - 3.1.3 替换玩家【" + player.ID + "】替换成功");
                EventManager.Send(MoatGameEvent.PlayerAdd, new object[]{ player.ID });
            }
            MDebug.LogFlow("3. 玩家进入 - 3.3 玩家Groups人数: "+ Players.Count);
        }

        public bool IsMaxPlayerCount()
        {
            return Players.Count >= MaxCount;
        }

        public VirtualPlayer GetVirtualPlayerById(string id)
        {
            foreach (var vPlayer in Players)
            {
                if (vPlayer.ID == id)
                {
                    return vPlayer;
                }
            }
            return null;
        }
        
        
        
        //=================================================================================
        
        public int Count = 1;

        public void SetNumberPlayer(int count)
        {
            Count = count;
            Players = new List<VirtualPlayer>();
        }

        public bool IsEnterRoi(int moveArea)
        {
            bool singleIsEnterRoi = MaxCount == 1 && moveArea > 0;
            if (singleIsEnterRoi)
            {
                return true;
            }

            bool multiIsEnterRoi = MaxCount > 1 && (0 < moveArea & moveArea < 5);
            if (multiIsEnterRoi)
            {
                return true;
            }

            return false;
        }

    }
}

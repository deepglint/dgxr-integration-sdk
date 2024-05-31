using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Player
{
    /// <summary>
    /// Represents the sprite in the game controlled by the player.
    /// </summary>
    /// <remarks>
    /// Character is a simple wrapper around the player, 
    /// </remarks>
    public interface ICharacter
    {
        /// <summary>
        /// When a player paired with a device which performed the join action is trying to join the game, 
        /// this "OnPlayerJoin" function will be invoked.
        /// When consenting to a player joining, return an ICharacter，if not, just return null.
        /// </summary>
        /// <param name="player">
        /// player is a instance instantiated from the PlayerManager.playerPrefab prefab.
        /// </param>
        /// <param name="device"></param>
        /// <returns> this </returns>
        public ICharacter OnPlayerJoin(GameObject player, InputDevice device);
        
        /// <summary>
        /// When the player join to current character is left this function will be called by the PlayerManager,
        /// after that the player will be destroyed by PlayerManager.
        /// </summary>
        public void OnPlayerLeft();
    }
    
    /// <summary>
    /// Represents the sprite in the game controlled by the player.
    /// </summary>
    /// <remarks>
    /// Character is a simple wrapper around the player,
    /// it is the data abstraction of the game sprite operated by the player within the game,
    /// which can be used to encapsulate information such as the identity and attributes of the game sprite.
    /// </remarks>
    public abstract class Character
    {
        internal Player m_Player;

        /// <summary>
        /// The player which controls current character
        /// </summary>
        public Player Player => m_Player;
        
        /// <summary>
        /// Name of current character
        /// </summary>
        public string Name;
        
        /// <summary>
        /// OnTryToJoin is a callback function.
        /// When a player which is paired with the device param is trying to join the game, 
        /// this "OnTryToJoin" function will be invoked.
        /// </summary>
        /// <param name="device"></param>
        /// <returns> this </returns>
        public abstract Character OnTryToJoin(InputDevice device);

        /// <summary>
        /// Check if there is a player which paired at least one main device already bind current character.
        /// </summary>
        protected bool IsBindable()
        {
            if (m_Player is null)
            {
                return true;
            }
            else
            {
                if (m_Player.m_PlayerInput is not null)
                {
                    // todo check the control schema.
                    if (m_Player.PairedDevices.Count == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}


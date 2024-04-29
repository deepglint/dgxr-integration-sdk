using UnityEngine.InputSystem;

namespace Deepglint.XR
{
    /// <summary>
    /// Represents the object in the game controlled by the player.
    /// </summary>
    /// <remarks>
    /// Character is a simple wrapper around the player, It represents the object in the game controlled by the player.
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
        protected internal string Name;
        
        /// <summary>
        /// OnJoin is a callback function.
        /// When a player which is paired with the device param is trying to join the game, 
        /// this "OnJoin" function will be invoked.
        /// </summary>
        /// <param name="device"></param>
        /// <returns> this </returns>
        public abstract Character OnJoin(InputDevice device);

        /// <summary>
        /// Check if there is a player which paired at least one main device already bind current character.
        /// </summary>
        public bool IsBindable()
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


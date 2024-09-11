using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Deepglint.XR.Player
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player : MonoBehaviour
    {
        internal Character m_Character;
        internal PlayerInput m_PlayerInput;
        
        /// <summary>
        /// The character controlled by current player. 
        /// </summary>
        public Character Character => m_Character;
        
        public ReadOnlyArray<InputDevice> PairedDevices 
        {
            get
            {
                if (m_PlayerInput is not null)
                {
                    return m_PlayerInput.devices;
                }

                return new ReadOnlyArray<InputDevice>();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public bool PairDeviceManually(InputDevice device)
        {
            Debug.Assert(m_PlayerInput is not null, "Only player initialized by PlayerManager can use PairDeviceManually function");
            
            return PlayerManager.Instance.PairDeviceToCharacter(m_Character, device);
        }

        /// <summary>
        /// Unpair a single device from current player.
        /// </summary>
        /// <param name="device">
        /// Device to unpair from the player.
        /// If the device is not currently paired to the user, the method does nothing.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="device"/> is <c>null</c>.</exception>
        public void UnPairDeviceManually(InputDevice device)
        {
            if (m_PlayerInput != null)
            {
                m_PlayerInput.user.UnpairDevice(device);
            }
        }

        private void OnDestroy()
        {
            if (m_PlayerInput != null)
            {
                // m_PlayerInput.user.UnpairDevices();
                DGXR.Logger.Log($"player {m_PlayerInput.user.id} was destoryed");
            }
        }
    }
}



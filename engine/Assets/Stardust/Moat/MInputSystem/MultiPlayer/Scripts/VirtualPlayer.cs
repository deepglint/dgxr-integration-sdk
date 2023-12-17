using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moat
{
    public enum PlayerInteractionState
    {
        NoEnter,
        Enter,
    }

    public class VirtualPlayer
    {
        public readonly string id;
        public bool isReady;
        public PlayerInteractionState state;
        public Vector2 movementInput;
        public Vector2 leftFootInput;
        public Vector2 rightFootInput;

        public VirtualPlayer(string playerId)
        {
            id = playerId;
            state = PlayerInteractionState.NoEnter;
            movementInput = new Vector2(1.5f, 1.5f);
            leftFootInput = new Vector2(1.5f, 1.5f);
            rightFootInput = new Vector2(1.5f, 1.5f);
        }

        public void Ready()
        {
            isReady = true;
        }
        
        public void Enter()
        {
            state = PlayerInteractionState.Enter;
        }

        public void NoEnter()
        {
            state = PlayerInteractionState.NoEnter;
        }

        public void Leave()
        {
            isReady = true;
        }
    }
}
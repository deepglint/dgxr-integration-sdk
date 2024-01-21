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
            movementInput = new Vector2(0f, 0f);
            leftFootInput = new Vector2(0f, 0f);
            rightFootInput = new Vector2(0f, 0f);
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

        public void Move(Vector2 position)
        {
            movementInput = position;
            leftFootInput = position;
            rightFootInput = position;
        }
    }
}
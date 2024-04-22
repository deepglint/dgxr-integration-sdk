using UnityEngine;

namespace Stardust.MultiPlayer.Scripts
{
    public enum PlayerInteractionState
    {
        NoEnter,
        Enter,
    }

    public class VirtualPlayer
    {
        public readonly string ID;
        public PlayerInteractionState State;
        public Vector2 MovementInput;
        public Vector2 LeftFootInput;
        public Vector2 RightFootInput;
        public float Angle;

        public VirtualPlayer(string id)
        {
            ID = id;
            State = PlayerInteractionState.NoEnter;
            MovementInput = new Vector2(1.5f, 1.5f);
            LeftFootInput = new Vector2(1.5f, 1.5f);
            RightFootInput = new Vector2(1.5f, 1.5f);
        }

        public void Enter()
        {
            State = PlayerInteractionState.Enter;
        }

        public void NoEnter()
        {
            State = PlayerInteractionState.NoEnter;
        }
    }
}
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Samples.Roam
{
    public struct Roi
    {
        public Vector2 Anchor;
        public float Radius;
    }
    
    public class CharacterManager: SingletonMono<MonoBehaviour>
    {
        public static AppCharacter MainCharacter;
        
        public void Start()
        {
            MainCharacter = new AppCharacter("漫游者", new Roi(){ Anchor = Vector2.zero, Radius = 1.0f });
            PlayerManager.Instance.OnTryToJoinWithCharacter += MainCharacter.OnTryToJoin;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Instantiate(transform.GetComponent<PlayerManager>().PlayerPrefab);
            }
        }
    }
    
    public class AppCharacter : Character
    {
        public readonly Roi Roi;
        public bool IsRealHuman;
        public DGXRHumanController Device;

        public AppCharacter(string name, Roi roi)
        {
            Name = name;
            Roi = roi;
        }

        public override Character OnTryToJoin(UnityEngine.InputSystem.InputDevice device)
        {
            Debug.LogFormat("device name: {0}", device.name);
            if (!IsBindable()) return null;
            IsRealHuman = device.name != "Keyboard";
            if (IsRealHuman) Device = (DGXRHumanController)device;
            return this;
        }
    }
}

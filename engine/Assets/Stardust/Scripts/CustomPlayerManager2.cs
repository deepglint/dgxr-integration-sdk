using UnityEngine;
using Deepglint.XR;
using Deepglint.XR.Inputs.Devices;
using UnityEngine.InputSystem;

// This demo demonstrates the process of multi player management.
public class CustomPlayerManager2 : MonoBehaviour
{
    private Character2 character1;
    private Character2 character2;
    private Character2 character3;

    public void Start()
    {
        character1 = new Character2("小红", new ROI(){ Anchor = new Vector2(-1, 0), Radius = 1.0f }); 
        character2 = new Character2("蓝蓝", new ROI(){ Anchor = Vector2.zero, Radius = 1.0f }); 
        character3 = new Character2("阿强", new ROI(){ Anchor = new Vector2(1, 0), Radius = 1.0f }); 
        PlayerManager.OnTryToJoin += character1.OnJoin;
        PlayerManager.OnTryToJoin += character2.OnJoin;
        PlayerManager.OnTryToJoin += character3.OnJoin;
    }
    
    public struct ROI
    {
        public Vector2 Anchor;
        public float Radius;
    }

    public class Character2 : Character
    {
        public ROI roi;
        public Character2(string name, ROI roi)
        {
            Name = name;
            this.roi = roi;
        }
        
        public override Character OnJoin(InputDevice device)
        {
            if (IsBindable())
            {
                if (device is DGXRController dgXRDevice)
                {
                    Vector3 position = dgXRDevice.HumanPose.position.ReadValue();
                    if (Vector2.Distance(roi.Anchor,new Vector2(position.x, position.z)) < roi.Radius)
                    {
                        Debug.LogFormat("character {0} is bindable", Name);
                        return this; 
                    }
                }
            }
            Debug.LogFormat("character {0} is not bindable", Name);
            return null;
        }
    }
}

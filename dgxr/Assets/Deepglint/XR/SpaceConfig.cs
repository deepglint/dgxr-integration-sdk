using Deepglint.XR;
using UnityEngine;

namespace Deepglint.XR 
{
    [ExecuteInEditMode]
    public class SpaceConfig : MonoBehaviour
    {
        void Start()
        {
            Global.SystemName = SystemInfo.operatingSystem; 
            Global.Config = new Config().InitConfig();
            MeshFilter[] display = GetComponentsInChildren<MeshFilter>();
            if (display.Length != Global.Config.Space.Screens.Count)
            {
                Debug.LogError("display and screen not match");
                return;
            }

            for (var i = 0; i < Global.Config.Space.Screens.Count; i++)
            {
                var position = Global.Config.Space.Screens[i].Position;
                var rotation = Global.Config.Space.Screens[i].Rotation;
                var size = Global.Config.Space.Screens[i].Size;
                display[i].transform.localPosition = new Vector3(position.x, position.y, position.z);
                display[i].transform.localRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, rotation.z));
                display[i].transform.localScale = new Vector3(size.x, size.y, size.z);
            }
        }
    }
}
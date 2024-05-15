using Deepglint.Tool.UIFrame;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deepglint.XR
{
    [ExecuteInEditMode]
    public class SpaceConfig : MonoBehaviour
    {
        [FormerlySerializedAs("UserViewCameraPrefab")]
        public Camera userViewCameraPrefab;

        [FormerlySerializedAs("ScreenPrefab")] public GameObject screenPrefab;

        private GameObject[] _screens;
        private Camera[] _cameras;
        private Vector3 _head = new Vector3(0, 1.6f, 0);

        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Global.SystemName = SystemInfo.operatingSystem;
                Global.Config = new Config().InitConfig();
                if (_cameras == null && _screens == null&&transform.childCount != 0)
                {
                    _cameras = new Camera[Global.Config.Space.Screens.Count];
                    _screens = new GameObject[Global.Config.Space.Screens.Count];
                    for (var i = 0; i < Global.Config.Space.Screens.Count; i++)
                    {
                        TargetDisplay target = (TargetDisplay)i;
                        var position = Global.Config.Space.Screens[i].Position;
                        var rotation = Global.Config.Space.Screens[i].Rotation;
                        var size = Global.Config.Space.Screens[i].Size;
                        _screens[i] = Instantiate(screenPrefab, transform);
                        _screens[i].transform.localPosition =
                            new Vector3(position.x, position.y, position.z);
                        _screens[i].transform.localRotation =
                            Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                        _screens[i].transform.localScale =
                            new Vector3(size.x, size.y, size.z);
                        _screens[i].name = target.ToString();
                        _cameras[i] = Instantiate(userViewCameraPrefab, transform.position,
                            _screens[i].transform.localRotation, _screens[i].transform);
                        _cameras[i].transform.position = _head;
                        _cameras[i].targetDisplay = Global.Config.Space.Screens[i].Display - 1;
                    }
                }
            }
#endif
        }

    }
}
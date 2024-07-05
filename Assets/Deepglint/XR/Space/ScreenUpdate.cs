using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Deepglint.XR.Space
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class ScreenUpdate
    {
        static ScreenUpdate()
        {
            DGXR.SystemName =  SystemInfo.operatingSystem;
            CheckResourceUpdates();
        }
        
        /// <summary>
        /// 检查并更新sdk 必要的资源
        /// </summary>
        private static void CheckResourceUpdates()
        {
            var config = new Config.Config().InitConfig();
            var cameraGroup = GameObject.Find("2DCameraGroup");
            var space = GameObject.Find("XRSpace");
            if (cameraGroup ==null || space == null)
            {
                return;
            }

            for (int i = 0; i < cameraGroup.transform.childCount; i++)
            {
                Transform camera = cameraGroup.transform.GetChild(i);
                var result =config.Space.Screens.FirstOrDefault(item => item.TargetScreen.ToString() == camera.name);
                camera.gameObject.SetActive(
                    !EqualityComparer<Config.Config.ScreenConfig>.Default.Equals(result, default));
            }
            for (int i = 0; i < space.transform.childCount; i++)
            {
                Transform spaceScreen = space.transform.GetChild(i);
                var resultScreen =config.Space.Screens.FirstOrDefault(item => item.TargetScreen.ToString() == spaceScreen.name);
                spaceScreen.gameObject.SetActive(
                    !EqualityComparer<Config.Config.ScreenConfig>.Default.Equals(resultScreen, default));
            }
        }
    }
#endif
}

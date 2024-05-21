using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Deepglint.XR
{
    [InitializeOnLoad]
    public static class ResourceUpdate
    {
        static ResourceUpdate()
        {
            Global.SystemName =  SystemInfo.operatingSystem;
            CheckResourceUpdates();
        }
        private static void CheckResourceUpdates()
        {
            var config = new Config().InitConfig();
            var cameraGroup = GameObject.Find("2DCameraGroup");
            var space = GameObject.Find("Space");
            if (cameraGroup ==null || space == null)
            {
                return;
            }

            for (int i = 0; i < cameraGroup.transform.childCount; i++)
            {
                Transform camera = cameraGroup.transform.GetChild(i);
                var result =config.Space.Screens.FirstOrDefault(item => item.Name == camera.name);
                camera.gameObject.SetActive(
                    !EqualityComparer<Config.ScreenInfo>.Default.Equals(result, default(Config.ScreenInfo)));
            }
            for (int i = 0; i < space.transform.childCount; i++)
            {
                Transform spaceScreen = space.transform.GetChild(i);
                var resultScreen =config.Space.Screens.FirstOrDefault(item => item.Name == spaceScreen.name);
                spaceScreen.gameObject.SetActive(
                    !EqualityComparer<Config.ScreenInfo>.Default.Equals(resultScreen, default(Config.ScreenInfo)));
            }
        }
    }
}
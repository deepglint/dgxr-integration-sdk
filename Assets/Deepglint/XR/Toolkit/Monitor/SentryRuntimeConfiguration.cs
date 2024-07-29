using System;
using Sentry.Unity;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Monitor
{
    [CreateAssetMenu(fileName = "Assets/Resources/Sentry/SentryRuntimeConfiguration.asset", menuName = "Sentry/SentryRuntimeConfiguration", order = 999)]
    public class SentryRuntimeConfiguration : SentryRuntimeOptionsConfiguration
    {
        
        /// <summary>
        /// 调用时机比Awake早，在Awake、Start赋值的变量无法获取
        /// </summary>
        /// <param name="options"></param>
        public override void Configure(SentryUnityOptions options)
        {
            var config = new Config.Config().InitConfig();
            Debug.Log("Sentry Env: " + config.Space.Name);
            options.Environment = config.Space.Name;
            options.CaptureInEditor = false;
            options.EnableLogDebouncing = true;
            options.Debug = true;
        }
    }
}

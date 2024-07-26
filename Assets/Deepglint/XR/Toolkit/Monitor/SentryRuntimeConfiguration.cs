using System;
using Sentry.Unity;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Monitor
{
    [CreateAssetMenu(fileName = "Assets/Resources/Sentry/SentryRuntimeConfiguration.asset", menuName = "Sentry/SentryRuntimeConfiguration", order = 999)]
    public class SentryRuntimeConfiguration : SentryRuntimeOptionsConfiguration
    {
        public override void Configure(SentryUnityOptions options)
        {
            options.Environment = DGXR.Config.Space.Name;
            options.CaptureInEditor = false;
            options.EnableLogDebouncing = true;
        }
    }
}

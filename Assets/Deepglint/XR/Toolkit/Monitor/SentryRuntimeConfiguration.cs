using Sentry.Unity;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Monitor
{
    [CreateAssetMenu(fileName = "Assets/Resources/Sentry/SentryRuntimeConfiguration.asset", menuName = "Sentry/SentryRuntimeConfiguration", order = 999)]
    public class SentryRuntimeConfiguration : SentryRuntimeOptionsConfiguration
    {

        public override void Configure(SentryUnityOptions options)
        {
            options.Environment = "延庆 & 9楼";
            options.CaptureInEditor = false;
        }
    }
}

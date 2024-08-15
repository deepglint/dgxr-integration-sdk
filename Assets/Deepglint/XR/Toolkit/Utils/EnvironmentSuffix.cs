using System.Collections.Generic;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public class EnvironmentSuffix
    {
        public static string GetEnvironment()
        {
            string version = Application.version;
            
            var environmentSuffixes = new Dictionary<string, string>
            {
                { "_test", "_test" },
                { "_dev", "_dev" }
            };

            foreach (var suffix in environmentSuffixes.Keys)
            {
                if (version.EndsWith(suffix))
                {
                    return environmentSuffixes[suffix];
                }
            }

            return version.StartsWith("v") ? "" : null;
        }
    }
}
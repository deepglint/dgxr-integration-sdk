using System.Collections.Generic;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public class EnvironmentSuffix
    {
        public static string GetEnvironment()
        {
            string version = Application.version;
            if (version.EndsWith("_test"))
            {
                return "_test";
            }

            if (version.EndsWith("_dev"))
            {
                return "_dev";
            }
            
            if (version.StartsWith("v"))
            {
                return "";
            }
            return null;
        }
    }
}
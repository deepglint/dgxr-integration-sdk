using Stardust.Model;
using UnityEngine;

namespace Stardust.Scripts
{
    public class MDebug: MSingleton<MDebug>
    {
        public static void LogTest(string value)
        {
            DisplayData.ReadConfig();
            int debugLevel = DisplayData.ConfigDisplay.DebugLevel;
            if (debugLevel >= 5)
            {
                Debug.Log(value); 
            }
        }

        public static void Log(string value)
        {
            DisplayData.ReadConfig();
            int debugLevel = DisplayData.ConfigDisplay.DebugLevel;
            if (debugLevel >= 4)
            {
                Debug.Log(value); 
            }
        }

        public static void Log(string value, string value1)
        {
            DisplayData.ReadConfig();
            int debugLevel = DisplayData.ConfigDisplay.DebugLevel;
            if (debugLevel >= 4)
            {
                Debug.Log(value + value1); 
            } 
        }

        public static void LogFlow(string value)
        {
            DisplayData.ReadConfig();
            int debugLevel = DisplayData.ConfigDisplay.DebugLevel;
            if (debugLevel >= 3)
            {
                Debug.Log("【Flow】" + value); 
            }
        }

        public static void LogWarning(string value)
        {
            DisplayData.ReadConfig();
            int debugLevel = DisplayData.ConfigDisplay.DebugLevel;
            if (debugLevel >= 2)
            {
                Debug.LogWarning(value); 
            }
        }

        public static void LogError(string value)
        {
            DisplayData.ReadConfig();
            int debugLevel = DisplayData.ConfigDisplay.DebugLevel;
            if (debugLevel >= 1)
            {
                Debug.LogError(value);
            }
        }
    }
}
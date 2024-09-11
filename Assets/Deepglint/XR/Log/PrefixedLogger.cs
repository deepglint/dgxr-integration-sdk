using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deepglint.XR.Log
{
    public class PrefixedLogger : ILogHandler
    {
        private ILogHandler _defaultLogHandler;
        private string _prefix;
        
        public PrefixedLogger(ILogHandler defaultLogHandler, string prefix)
        {
            _defaultLogHandler = defaultLogHandler;
            _prefix = prefix;
        }
        
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string prefixedFormat = $"[{_prefix}] {format}";
            _defaultLogHandler.LogFormat(logType, context, prefixedFormat, args);
        }

        public void LogException(Exception exception, Object context)
        {
            _defaultLogHandler.LogException(exception, context);
        }
    }
}
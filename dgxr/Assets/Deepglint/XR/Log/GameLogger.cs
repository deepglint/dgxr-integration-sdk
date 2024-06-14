using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Deepglint.XR.Log
{
    public static class GameLogger
    {
        private static bool _debugLogEnable = true;
        private static bool _warningLogEnable = true;
        private static readonly StringBuilder LogStr = new StringBuilder();
        private static string _logFileSavePath;
        private static readonly object FileLock = new object();
        private static Timer _logFlushTimer;

        public static void Init(Config.Config.ConfigData.LogInfo logInfo)
        {
            switch (logInfo.Level)
            {
                case "debug":
                    break;
                case "warn":
                    _debugLogEnable = false;
                    break;
                case "error":
                    _debugLogEnable = false;
                    _warningLogEnable = false;
                    break;
            }

            if (!Global.SystemName.Contains("Mac"))
            {
                CreateLogFile();
                Application.logMessageReceived += OnLogCallBack;
                _logFlushTimer = new Timer(FlushLogToFile, null, 5000, 5000);
            }
        }

        public static void CreateLogFile()
        {
            var t = DateTime.Now.ToString("yyyyMMddhhmmss");

            if (!string.IsNullOrEmpty(Global.Config.Log.SavePath))
            {
                _logFileSavePath =
                    $"{Global.Config.Log.SavePath}/{Global.AppName}/DGXR_{Global.Version}_{t}.log";
            }
            else
            {
                _logFileSavePath = $"{Application.persistentDataPath}/{Global.AppName}/DGXR_{Global.Version}_{t}.log";
            }

            var logDirectory = Path.GetDirectoryName(_logFileSavePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            if (LogStr.Length > 0)
            {
                if (!File.Exists(_logFileSavePath))
                {
                    var fs = File.Create(_logFileSavePath);
                    fs.Close();
                }
            }
        }

        private static void OnLogCallBack(string condition, string stackTrace, LogType type)
        {
            if ((type == LogType.Log && !_debugLogEnable) || (type == LogType.Warning && !_warningLogEnable))
            {
                return;
            }

            lock (LogStr)
            {
                string logMessage = $"{DateTime.Now} - {condition}\n{stackTrace}";
                LogStr.Append(logMessage);
            }
        }

        private static void FlushLogToFile(object state)
        {
            lock (FileLock)
            {
                if (LogStr.Length > 0)
                {
                    var logsToWrite = new StringBuilder();

                    lock (LogStr)
                    {
                        logsToWrite.Append(LogStr.ToString());
                        LogStr.Clear();
                    }

                    Task.Run(() =>
                    {
                        lock (FileLock)
                        {
                            using (var sw = File.AppendText(_logFileSavePath))
                            {
                                sw.Write(logsToWrite.ToString());
                            }
                        }
                    });

                    CheckLogFileSize();
                }
            }
        }

        private static void CheckLogFileSize()
        {
            lock (FileLock)
            {
                string logFolderPath = Path.GetDirectoryName(_logFileSavePath);
                if (logFolderPath != null)
                {
                    string[] logFiles = Directory.GetFiles(logFolderPath, "*.log");
                    DateTime currentTime = DateTime.Now;

                    foreach (string filePath in logFiles)
                    {
                        DateTime creationTime = File.GetCreationTime(filePath);
                        TimeSpan age = currentTime - creationTime;
                        if (age.TotalDays > Global.Config.Log.SaveDay)
                        {
                            File.Delete(filePath);
                        }
                    }
                }

                long maxFileSize = 1024 * 1024 * Global.Config.Log.SingFileMaxSize;
                FileInfo logFileInfo = new FileInfo(_logFileSavePath);

                if (logFileInfo.Exists && logFileInfo.Length > maxFileSize)
                {
                    CreateLogFile();
                }
            }
        }

        // 确保在应用程序退出时清理资源
        public static void Cleanup()
        {
            _logFlushTimer?.Dispose();
            FlushLogToFile(null);
            if (!Global.SystemName.Contains("Mac"))
            {
                Application.logMessageReceived -= OnLogCallBack;
            } 
        }
    }
}

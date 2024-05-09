using System;
using UnityEngine;
using System.Text;
using System.IO;

namespace Runtime.Scripts.Log
{
    public class GameLogger
    {
        // 普通调试日志开关
        private static bool _debugLogEnable = true;

        // 警告日志开关
        private static bool _warningLogEnable = true;

        // 使用StringBuilder来优化字符串的重复构造
        private static readonly StringBuilder LogStr = new StringBuilder();

        // 日志文件存储位置
        private static string _logFileSavePath;

        /// <summary>
        /// 初始化，在游戏启动的入口脚本的Awake函数中调用GameLogger.Init
        /// </summary>
        public static void Init(Config.ConfigData.LogInfo logInfo)
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
                CreatLogFile();
                Application.logMessageReceived += OnLogCallBack;
            }
        }

        public static void CreatLogFile()
        {
            var t = DateTime.Now.ToString("yyyyMMddhhmmss");

            if (!string.IsNullOrEmpty(Global.Config.Log.SavePath))
            {
                _logFileSavePath =
                    $"{Global.Config.Log.SavePath}/{Global.AppName}/DGXR_{t}.log";
            }
            else
            {
                _logFileSavePath = $"{Application.persistentDataPath}/{Global.AppName}/DGXR_{t}.log";
            }

            if (!Directory.Exists(Path.GetDirectoryName(_logFileSavePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFileSavePath) ?? string.Empty);
            }

            if (LogStr.Length <= 0) return;
            if (!File.Exists(_logFileSavePath))
            {
                var fs = File.Create(_logFileSavePath);
                fs.Close();
            }
        }

        /// <summary>
        /// 打印日志回调
        /// </summary>
        /// <param name="condition">日志文本</param>
        /// <param name="stackTrace">调用堆栈</param>
        /// <param name="type">日志类型</param>
        private static void OnLogCallBack(string condition, string stackTrace, LogType type)
        {
            if ((type == LogType.Log && !_debugLogEnable) || (type == LogType.Warning && !_warningLogEnable))
            {
                return;
            }

            CheckLogFileSize();
            string logMessage = $"{DateTime.Now} - {condition}\n{stackTrace}\n";
            LogStr.Append(logMessage);

            using (var sw = File.AppendText(_logFileSavePath))
            {
                sw.WriteLine(LogStr.ToString());
            }

            LogStr.Remove(0, LogStr.Length);
        }

        private static void CheckLogFileSize()
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
                CreatLogFile();
            }
        }
    }
}
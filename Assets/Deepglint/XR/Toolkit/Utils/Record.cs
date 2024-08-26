using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public class Record
    {
        private readonly string _path;
        private int _currentHouse = -1;
        private string _marker;
        private StreamWriter _writer;
        private readonly object _lock = new object();

        public Record(string path, string marker)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _path = path;
            }
            else
            {
                _path = Path.Combine(Application.persistentDataPath, "records");
            }

            _marker = marker;
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            CheckOldFile();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public async Task SaveMsgData(string msg)
        {
            lock (_lock)
            {
                DateTime currentTime = DateTime.Now;
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour,
                    currentTime.Minute, currentTime.Second);
                string timestamp = currentTime.ToString("yyyyMMddHHmmss");
                string filePath = Path.Combine(_path, $"{DGXR.AppName}_{DGXR.AppVersion}_{timestamp}_{_marker}.txt");
                if (_currentHouse == -1)
                {
                    CheckFile(filePath);
                    _writer = new StreamWriter(filePath, true);
                    _currentHouse = currentTime.Hour;
                }

                if (_currentHouse != currentTime.Hour)
                {
                    currentTime =  DateTime.Now;
                    string oldTimestamp = currentTime.ToString("yyyyMMddHHmmss");
                    string newFilePath =
                        Path.Combine(_path, $"{DGXR.AppName}_{DGXR.AppVersion}_{oldTimestamp}_{_marker}.txt");
                    CheckFile(newFilePath);
                    _writer.Close();
                    _writer = new StreamWriter(newFilePath, true);
                    _currentHouse = currentTime.Hour;
                }
                _writer.WriteLine(msg);
            }
        }

        private void CheckFile(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        private void CheckOldFile()
        {
            // 检查旧文件删除
            DateTime currentTime = DateTime.Now;
            string[] recordFiles = Directory.GetFiles(_path, "*.txt");
            foreach (string file in recordFiles)
            {
                DateTime creationTime = File.GetCreationTime(file);
                TimeSpan age = currentTime - creationTime;
                if (age.TotalDays > DGXR.Config.Record.SaveDay)
                {
                    File.Delete(file);
                }
            }
        }
    }
}
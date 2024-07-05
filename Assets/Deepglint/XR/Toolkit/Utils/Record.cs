using System;
using System.IO;

namespace Deepglint.XR.Toolkit.Utils
{
    public class Record
    {
        private readonly string _path;
        private int _currentHouse=-1;
        private string _marker;
        private StreamWriter _writer;

        public Record(string path, string marker)
        {
            _path = path;
            _marker = marker;
            if (!DGXR.SystemName.Contains("Mac"))
            {
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }

                CheckOldFile();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SaveMsgData(object data)
        {
            string msg = (string)data;
            DateTime currentTime = DateTime.Now;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0);
            string fileName = currentTime.ToString("yyyyMMddHHmmss");
            string filePath = Path.Combine(_path, fileName + "_"+_marker+".txt");
            if (_currentHouse == -1)
            {
                CheckFile(filePath);
                _writer = new StreamWriter(filePath, true);
                _currentHouse = currentTime.Hour;
            }
            
            if (_currentHouse!=currentTime.Hour)
            {
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, _currentHouse, 0, 0);
                string oldFileName = currentTime.ToString("yyyyMMddHHmmss");
                string oldFilePath = Path.Combine(_path, oldFileName + "_"+_marker+".txt");
                CheckFile(oldFilePath);
                _writer.Close();
                _writer = new StreamWriter(oldFilePath, true);
                _currentHouse = currentTime.Hour;
            }
            _writer.WriteLine(msg); 
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
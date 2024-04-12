using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Stardust.log;
using UnityEngine;
using UnityEngine.Playables;

namespace DGXR
{
    public class Record
    {
        private List<string> _frameDataList = new List<string>();
        private readonly int _maxFrameDataPerSecond = 60;
        private readonly string _path;
        private int _currentHouse=-1;
        private string _marker;
        private StreamWriter _writer;

        public Record(string path,int frameSize, string marker)
        {
            _path = path;
            _maxFrameDataPerSecond = frameSize;
            _marker = marker;
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            DateTime currentTime = DateTime.Now;
            
            
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
                string oldfilePath = Path.Combine(_path, oldFileName + "_"+_marker+".txt");
                CheckFile(oldfilePath);
                _writer.Close();
                _writer = new StreamWriter(oldfilePath, true);
                SaveFrameDataToFile(oldfilePath);
                _frameDataList.Clear();
                _currentHouse = currentTime.Hour;
            }
            _frameDataList.Add(msg);
            if (_frameDataList.Count < _maxFrameDataPerSecond) return;
            SaveFrameDataToFile(filePath);
            _frameDataList.Clear();
        }

        private void CheckFile(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        private void SaveFrameDataToFile(string filePath)
        {
            foreach (var frameData in _frameDataList)
            {
                _writer.WriteLine(frameData);
            }
            // 检查旧文件删除
            DateTime currentTime = DateTime.Now;
            string[] recordFiles = Directory.GetFiles(_path, "*.txt");
            foreach (string file in recordFiles)
            {
                DateTime creationTime = File.GetCreationTime(file);
                TimeSpan age = currentTime - creationTime;
                if (age.TotalDays > Global.Config.Record.SaveDay)
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
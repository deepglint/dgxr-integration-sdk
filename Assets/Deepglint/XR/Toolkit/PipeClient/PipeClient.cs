using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.PipeClient
{
    public class PipeClient : MonoBehaviour
    {
        public enum PipeEvent
        {
            ApplicationClosed
        }

#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        private ManualResetEvent _messageSentEvent = new ManualResetEvent(false);

        private void Start()
        {
            AppExit.OnAppExit += OnAppExit;
        }

        private void OnAppExit()
        {
            Thread clientThread = new Thread(() =>
            {
                try
                {
                    using (NamedPipeClientStream pipeClient =
                           new NamedPipeClientStream(".", $"meta-starter{EnvironmentSuffix.GetEnvironment()}",
                               PipeDirection.Out))
                    {
                        pipeClient.Connect();
                        DGXR.Logger.Log($"Pipe client meta-starter{EnvironmentSuffix.GetEnvironment()} connected");

                        using (StreamWriter writer = new StreamWriter(pipeClient))
                        {
                            writer.AutoFlush = true;
                            writer.WriteLine(PipeEvent.ApplicationClosed);
                        }
                    }
                    _messageSentEvent.Set();
                }
                catch (Exception e)
                {
                    DGXR.Logger.LogError("PipeClient", $"SendPipeMessage Exception: {e.Message}");
                    _messageSentEvent.Set();    
                }
            });

            clientThread.Start();
            _messageSentEvent.WaitOne(500);
            _messageSentEvent.Dispose();
            clientThread.Abort();
        }

#endif
    }
}
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
#if !UNITY_EDITOR
        private void Start()
        {
            AppExit.OnAppExit += OnAppExit;
        }

        private void OnDestroy()
        {
            AppExit.OnAppExit -= OnAppExit;
        }


        private void OnAppExit()
        {
            Thread clientThread = new Thread(() =>
            {
                try
                {
                    using (NamedPipeClientStream pipeClient =
 new NamedPipeClientStream(".", $"meta-starter{EnvironmentSuffix.GetEnvironment()}", PipeDirection.Out))
                    {
                        pipeClient.Connect();
                        Debug.Log($"Pipe client meta-starter{EnvironmentSuffix.GetEnvironment()} connected");

                        using (StreamWriter writer = new StreamWriter(pipeClient))
                        {
                            writer.AutoFlush = true;
                            writer.WriteLine("ApplicationClosed");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"SendPipeMessage Exception: {e.Message}");
                }
            });

            clientThread.Start();
        }
#endif
    }
}
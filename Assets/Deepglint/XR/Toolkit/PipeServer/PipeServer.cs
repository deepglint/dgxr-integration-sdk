using System.IO;
using System.IO.Pipes;
using UnityEngine;

namespace Deepglint.XR.Toolkit.PipeServer
{
    public class PipeServer : MonoBehaviour
    {
#if !UNITY_EDITOR
        private void OnApplicationQuit()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "meta-starter", PipeDirection.Out))
            {
                pipeClient.Connect();
                Debug.Log("Pipe client connected");
                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine("ApplicationClosed");
                }
            }
        }
#endif
    }
}
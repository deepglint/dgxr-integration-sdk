using System;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Deepglint.XR.Toolkit.DebugTool
{
    public class SingleScreen : MonoBehaviour
    {
        private void Start()
        {
            if (Global.Config.SingleScreen)
            {
                Screen.SetResolution(1920, 1200, true);
                Camera cam = gameObject.AddComponent<Camera>();
                cam.cullingMask = 0;
                cam.depth = -100;
                cam.clearFlags = CameraClearFlags.Color;
                cam.backgroundColor = Color.black;
                
                float height = 1200f;
                float width = 1920f;
               
                float partWidth = width / 15f;
                float edgeBorder = height-partWidth/1.6f*13f; 
                float partHeight = (height -edgeBorder)/13f; 
                
                float ratioHeight = partHeight * 5f / height;
                float ratioBottomHeight = partHeight * 8f / height;
                float ratioWidth =partWidth*5f/width;

                float leftX = 0;
                float midX = partWidth*5f / width;
                float rightX = 2f*partWidth*5f/ width;
                float topY = (8f * partHeight+edgeBorder) / height;
                float midY = 1-(10f * partHeight) / height;
                float bottomY = edgeBorder/height;

               
                foreach (var space in Global.Space)
                {
                    space.SpaceCamera.targetDisplay = 0;

                    foreach (var overCam in space.SpaceCamera.GetUniversalAdditionalCameraData().cameraStack)
                    {
                        overCam.targetDisplay = 0;
                    }
                    
                    switch (space.TargetScreen)
                    {
                        case TargetScreen.Front:
                            space.SpaceCamera.rect = new Rect(midX, topY, ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Right:
                            space.SpaceCamera.rect = new Rect(rightX, topY, ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Back:
                            space.SpaceCamera.rect = new Rect(rightX, midY, ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Left:
                            space.SpaceCamera.rect = new Rect(leftX, topY,ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Bottom:
                            space.UICamera.orthographicSize = 600;
                            space.SpaceCamera.rect = new Rect(midX, bottomY, ratioWidth, ratioBottomHeight);
                            break;
                    }
                }
            }
        }
    }
}
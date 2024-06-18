using System;
using Deepglint.XR.Space;
using UnityEngine;

namespace Deepglint.XR.Toolkit.DebugTool
{
    public class SingleScreen : MonoBehaviour
    {
        private void Start()
        {
            if (Global.Config.SingleScreen)
            {
                Screen.SetResolution(1920, 1200, true);

                float height = 1200f;
                float width = 1920f;
                
                float partHeight = height / 18f;
                float edgeBorder = (width-partHeight * 8f * 3f)/2f;
                float partWidth = (width - 2 * edgeBorder) / 3;

                float ratioHeight = partHeight * 5f / height;
                float ratioBottomHeight = partHeight * 8f / height;
                float ratioWidth = partWidth / width;

                float leftX = edgeBorder / width;
                float midX = (edgeBorder + partWidth) / width;
                float rightX = (edgeBorder + 2*partWidth) / width;
                float topY = (13 * partHeight) / height;
                float midY = (5 * partHeight) / height;
                float bottomY = 0;
             
                foreach (var space in Global.Space)
                {
                    space.SpaceCamera.targetDisplay = 0;
                    switch (space.TargetScreen)
                    {
                        case TargetScreen.Front:
                            space.SpaceCamera.rect = new Rect(midX, topY, ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Right:
                            space.SpaceCamera.rect = new Rect(rightX, topY, ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Back:
                            space.SpaceCamera.rect = new Rect(midX, bottomY, ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Left:
                            space.SpaceCamera.rect = new Rect(leftX, topY,ratioWidth, ratioHeight);
                            break;
                        case TargetScreen.Bottom:
                            space.SpaceCamera.rect = new Rect(midX, midY, ratioWidth, ratioBottomHeight);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
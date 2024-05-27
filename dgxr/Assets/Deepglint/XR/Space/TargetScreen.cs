using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Deepglint.XR.Space
{
    public enum TargetScreen
    {
        Front,
        Right,
        Back,
        Left,
        Bottom,
    }

    public class ScreenInfo : Config.Config.ScreenConfig
    {
        public ScreenInfo(Config.Config.ScreenConfig config)
        {
            TargetScreen = config.TargetScreen;
            Render = config.Render;
            Position = config.Position;
            Rotation = config.Rotation;
            Size = config.Size;
        }
        public GameObject ScreenObject { get; internal set; }
        public Camera UICamera { get; internal set; }

        public Camera SpaceCamera { get; internal set; }

        public Resolution Resolution { get; internal set; }
        public GameObject ScreenCanvas { get; internal set; }

        public void AddCameraToStack(Camera camera)
        {
            if (SpaceCamera == null || camera == null)
            {
                Debug.LogError("SpaceCamera or the camera to be added is null.");
                return;
            }

            UniversalAdditionalCameraData spaceCameraData = SpaceCamera.GetUniversalAdditionalCameraData();

            if (spaceCameraData == null)
            {
                Debug.LogError("SpaceCamera does not have UniversalAdditionalCameraData component.");
                return;
            }

            if (camera.GetUniversalAdditionalCameraData() == null)
            {
                Debug.LogError("The camera to be added does not have UniversalAdditionalCameraData component.");
                return;
            }


            if (!spaceCameraData.cameraStack.Contains(camera))
            {
#if !UNITY_EDITOR
                if (TargetScreen.Bottom == TargetScreen)
                {
                    var rotation = camera.transform.localRotation;
                    camera.transform.rotation = Quaternion.Euler(rotation.x	, rotation.y, rotation.z+(int)Rotation.z);
                }
#endif
                spaceCameraData.cameraStack.Add(camera);
            }
            else
            {
                Debug.LogWarning("Camera is already in the stack.");
            }
        }

        /// <summary>
        /// Removes a camera from the SpaceCamera's stack.
        /// </summary>
        /// <param name="camera">The camera to remove.</param>
        public void RemoveCameraFromStack(Camera camera)
        {
            if (SpaceCamera == null || camera == null)
            {
                Debug.LogError("SpaceCamera or the camera to be removed is null.");
                return;
            }

            UniversalAdditionalCameraData spaceCameraData = SpaceCamera.GetUniversalAdditionalCameraData();
            if (spaceCameraData == null)
            {
                Debug.LogError("SpaceCamera does not have UniversalAdditionalCameraData component.");
                return;
            }

            if (spaceCameraData.cameraStack.Contains(camera))
            {
                spaceCameraData.cameraStack.Remove(camera);
            }
            else
            {
                Debug.LogWarning("Camera is not in the stack.");
            }
        }



        public Vector2 ProjectionVector3(Vector3 point)
        {
            return ProjectionVector3(point, this);
        }

        public float DistanceToScreen(Vector3 point)
        {
            return DistanceToScreen(point, this);
        }

        public Vector2 SpaceToPixelOnScreen(Vector2 point)
        {
            return SpaceToPixelOnScreen(point, this);
        }

        public Vector2 BottomRelativeToScreen(Vector2 point)
        {
            return BottomRelativeToScreen(point, this);
        }

        public bool RayTo(Ray ray, out Vector2 intersection)
        {
            return RayTo(ray, this, out intersection);
        }

        public static Vector2 ProjectionVector3(Vector3 point, ScreenInfo screen)
        {
            return ProjectionVector3(point, screen.TargetScreen);
        }



        public static Vector2 ProjectionVector3(Vector3 point, TargetScreen screen)
        {
            var res = screen switch
            {
                TargetScreen.Front => new Vector2(point.x, point.y),
                TargetScreen.Back => new Vector2(-point.x, point.y),
                TargetScreen.Left => new Vector2(point.z, point.y),
                TargetScreen.Right => new Vector2(-point.z, point.y),
                TargetScreen.Bottom => new Vector2(point.x, point.z),
                _ => throw new ArgumentOutOfRangeException(nameof(screen), screen, null)
            };
            return res;
        }


        public static float DistanceToScreen(Vector3 point, ScreenInfo screen)
        {
            int  zRange = screen.Resolution.height / 2;
            int xRange = screen.Resolution.width / 2;

            float res = screen.TargetScreen switch
            {
                TargetScreen.Front => zRange - point.z,
                TargetScreen.Back => zRange + point.z,
                TargetScreen.Left => xRange - point.x,
                TargetScreen.Right => xRange + point.x,
                TargetScreen.Bottom => point.y,
                _ => throw new ArgumentOutOfRangeException(nameof(screen), screen, null)
            };
            return res;
        }


        // TODO: 用vector3
        public static Vector2 SpaceToPixelOnScreen(Vector2 spacePosition, ScreenInfo screen)
        {
            float xRatio = screen.Resolution.width/ screen.Size.x;
            float yRatio = screen.Resolution.height / screen.Size.z;

            if (screen.TargetScreen == TargetScreen.Bottom)
            {
                yRatio = screen.Resolution.width/ screen.Size.x;

            }
            else
            {
                // 真实空间z轴起点在空间地面中心，而不是空间的几何中心，几何中心在空中，不好对齐和使用
                spacePosition.y -= screen.Size.z/2;
            }

            int x = Mathf.RoundToInt(spacePosition.x * xRatio);
            int y = Mathf.RoundToInt(spacePosition.y * yRatio);

            return new Vector2(x, y);
        }

        // 参数 position 为地屏与当前侧屏相交线中点为原点的坐标
        public static Vector2 BottomRelativeToScreen(Vector2 position, ScreenInfo screen)
        {
            // 前提是地屏长与宽分辨率相等（实际长度可以不等，但是最终拼接的分辨率长宽相等，等于侧屏分辨率）
            int baseline = screen.Resolution.width /2;
            return screen.TargetScreen switch
            {
                TargetScreen.Front => new Vector2(position.x, baseline - position.y),
                TargetScreen.Left => new Vector2(position.y - baseline, -position.x),
                TargetScreen.Right => new Vector2(baseline - position.y, -position.x),
                TargetScreen.Back => new Vector2(-position.x, position.y - baseline),
                TargetScreen.Bottom => position,
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        public static bool RayTo(Ray ray, ScreenInfo screen, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            MeshCollider meshCollider = screen.ScreenObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = screen.ScreenObject.AddComponent<MeshCollider>();
            }
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == meshCollider)
            {
                intersection = hit.point;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return TargetScreen.ToString();
        }
    }


}

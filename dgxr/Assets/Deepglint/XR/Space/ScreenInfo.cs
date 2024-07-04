using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Deepglint.XR.Space
{
    /// <summary>
    /// 屏幕位置与顺序, 与Unity Camera 及 Canvas上的 targetDisplay 对应
    /// 一般人从门进入之后，正对的是前面屏，其他屏幕按此时的站立方向取值
    /// 屏幕是渲染的单位，与实际接入
    /// </summary>
    public enum TargetScreen
    {
        /// <summary>
        /// 前屏幕，
        /// </summary>
        Front,
        
        /// <summary>
        /// 右侧屏幕
        /// </summary>
        Right,
        
        /// <summary>
        /// 后侧屏幕
        /// </summary>
        Back,
        
        /// <summary>
        /// 左侧屏幕
        /// </summary>
        Left,
        
        /// <summary>
        /// 地面屏幕，地面屏幕
        /// </summary>
        Bottom,
    }

    /// <summary>
    /// ScreenInfo 用来描述空间对应的投影屏幕信息，并提供了关联的UI Canvas，Camera 及和屏幕位置相关的工具方法
    /// </summary>
    public class ScreenInfo : Config.Config.ScreenConfig
    {
        /// <summary>
        /// 初始化屏幕信息
        /// </summary>
        /// <param name="config">屏幕配置</param>
        internal ScreenInfo(Config.Config.ScreenConfig config)
        {
            TargetScreen = config.TargetScreen;
            Render = config.Render;
            Position = config.Position;
            Rotation = config.Rotation;
            Scale = config.Scale;
        }

        /// <summary>
        /// 空间中的屏幕对象
        /// </summary>
        public GameObject ScreenObject { get; internal set; }
        
        /// <summary>
        /// 拍摄这个屏幕的UI相机
        /// </summary>
        public Camera UICamera { get; internal set; }

        /// <summary>
        /// 拍摄这个屏幕方向的3D相机
        /// </summary>
        public Camera SpaceCamera { get; internal set; }

        /// <summary>
        /// 分辨率，因为分辨率统一要求，该分辨率可能与Display下实际的分辨率不一致
        /// </summary>
        public Resolution Resolution { get; internal set; }
        
        /// <summary>
        /// 屏幕的Canvas
        /// </summary>
        public GameObject ScreenCanvas { get; internal set; }
        
        /// <summary>
        /// 屏幕关联的Unity Display 
        /// </summary>
        public Display Display => Display.displays[(int)TargetScreen];

        /// <summary>
        /// 屏幕实际大小，单位：m
        /// </summary>
        public Vector2 Size { get; internal set; }

        /// <summary>
        /// 增加相机到当前渲染屏幕
        /// </summary>
        /// <param name="camera">相机对象</param>
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
                spaceCameraData.cameraStack.Add(camera);
            }
            else
            {
                Debug.LogWarning("Camera is already in the stack.");
            }
        }

        /// <summary>
        /// 移除相机到当前渲染屏幕
        /// </summary>
        /// <param name="camera">移除的相机对象</param>
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


        /// <summary>
        /// 将实体空间中的3维坐标投影到当前屏幕
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 ProjectionVector3(Vector3 point)
        {
            return ProjectionVector3(point, this);
        }
        
        /// <summary>
        /// 计算实体空间中的3维坐标到当前屏幕的物理距离
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float DistanceToScreen(Vector3 point)
        {
            return DistanceToScreen(point, this);
        }

        /// <summary>
        /// 将实体空间投影到当前屏幕的坐标换算为当前这块屏幕在分辨率下坐标，返回的坐标原点为屏幕中心
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 SpaceToPixelOnScreen(Vector2 point)
        {
            return SpaceToPixelOnScreen(point, this);
        }

        /// <summary>
        /// 将实体空间坐标先投影到当前屏幕，然后换算为当前屏幕在分辨率下的坐标，返回的坐标原点为屏幕中心
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 SpaceToPixelOnScreen(Vector3 point)
        {
            return SpaceToPixelOnScreen(point, this);
        }

        /// <summary>
        /// 计算相对于当前屏幕底边中点的某个像素坐标，在地面屏幕上的换算像素坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 BottomRelativeToScreen(Vector2 point)
        {
            return BottomRelativeToScreen(point, this);
        }

        /// <summary>
        /// 将空间3维坐标投影到某个屏幕
        /// </summary>
        /// <param name="point"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 计算实体空间中的3维坐标到某个屏幕的物理距离
        /// </summary>
        /// <param name="point">坐标点</param>
        /// <param name="screen">屏幕</param>
        /// <returns>物理距离</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float DistanceToScreen(Vector3 point, ScreenInfo screen)
        {
            int zRange = screen.Resolution.height / 2;
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

        /// <summary>
        /// 将空间坐标投影到对应屏幕上，然后计算屏幕上的像素坐标，像素坐标原点为屏幕中心
        /// </summary>
        /// <param name="spacePosition">空间坐标</param>
        /// <param name="screen">屏幕</param>
        /// <returns>映射后的像素坐标</returns>
        public static Vector2 SpaceToPixelOnScreen(Vector3 spacePosition, ScreenInfo screen)
        {
            var position = ProjectionVector3(spacePosition, screen);
            return SpaceToPixelOnScreen(position, screen);
        }

        /// <summary>
        /// 将投影到对应屏幕的二维空间坐标映射为屏幕上的像素坐标，像素坐标原点为屏幕中心
        /// </summary>
        /// <param name="spacePosition"></param>
        /// <param name="screen"></param>
        /// <returns>映射后的像素坐标</returns>
        public static Vector2 SpaceToPixelOnScreen(Vector2 spacePosition, ScreenInfo screen)
        {
            float xRatio = screen.Resolution.width / screen.Size.x;
            float yRatio = screen.Resolution.height / screen.Size.y;

            if (screen.TargetScreen == TargetScreen.Bottom)
            {
                yRatio = screen.Resolution.width / screen.Size.x;
            }
            else
            {
                // 真实空间z轴起点在空间地面中心，而不是空间的几何中心，几何中心在空中，不好对齐和使用
                spacePosition.y -= screen.Size.y / 2;
            }

            int x = Mathf.RoundToInt(spacePosition.x * xRatio);
            int y = Mathf.RoundToInt(spacePosition.y * yRatio);

            return new Vector2(x, y);
        }

        /// <summary>
        /// 计算相对于当前屏幕底边中点的某个像素坐标，在地面屏幕上的换算像素坐标
        /// </summary>
        /// <param name="position">地屏与当前侧屏相交线中点为原点的坐标</param>
        /// <param name="screen">屏幕</param>
        /// <returns>映射后在地屏上的像素坐标</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Vector2 BottomRelativeToScreen(Vector2 position, ScreenInfo screen)
        {
            // 前提是地屏长与宽分辨率相等（实际长度可以不等，但是最终拼接的分辨率长宽相等，等于侧屏分辨率）
            int baseline = screen.Resolution.width / 2;
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
        

        public override string ToString()
        {
            return TargetScreen.ToString();
        }
    }
}
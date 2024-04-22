using Stardust.Model;
using Stardust.Scripts;
using UnityEngine;

namespace Stardust.MultiPlayer.Scripts
{
    public class RoiTools : MSingleton<RoiTools>
    {
        private float[][] _interactionAreaPosition = new float[][]
        {
            new [] { 0f, 0f, 0f },
            new [] { 0f, 0f, 0f },
            new [] { 0f, 0f, 0f },
            new [] { 0f, 0f, 0f },
            new [] { 0f, 0f, 0f },
        };

        public float GetTestPosX(int area)
        {
            if (area > 0 && area <= 5)
            {
                return (_interactionAreaPosition[area - 1][0] + _interactionAreaPosition[area - 1][1]) / 2;
            }

            return 10f;
        }

        public int CheckEnterArea(Vector2 leftFootPos)
        {
            if (EnterArea(leftFootPos, _interactionAreaPosition[0]))
            {
                return 1;
            }

            if (EnterArea(leftFootPos, _interactionAreaPosition[1]))
            {
                return 2;
            }

            if (EnterArea(leftFootPos, _interactionAreaPosition[2]))
            {
                return 3;
            }

            if (EnterArea(leftFootPos, _interactionAreaPosition[3]))
            {
                return 4;
            }

            if (EnterArea(leftFootPos, _interactionAreaPosition[4]))
            {
                return 5;
            }

            return 0;
        }

        public bool EnterArea(Vector2 leftFootPos, float[] areaPosition)
        {
            if (leftFootPos.x >= areaPosition[0] && leftFootPos.x < areaPosition[1] &&
                leftFootPos.y >= -areaPosition[2] && leftFootPos.y <= areaPosition[2])
            {
                return true;
            }

            return false;
        }

        private Vector2 _p1, _p2, _p3, _p4;

        // roi边界检测
        public bool CheckBoundary(Vector2 p)
        {
            float cameraRoi = DisplayData.ConfigDisplay.Resolution.RealResolution;

            if (_p1.Equals(Vector2.zero)) _p1 = new Vector2(-cameraRoi * 0.5f, cameraRoi * 0.5f);
            if (_p2.Equals(Vector2.zero)) _p2 = new Vector2(cameraRoi * 0.5f, cameraRoi * 0.5f);
            if (_p3.Equals(Vector2.zero)) _p3 = new Vector2(cameraRoi * 0.5f, -cameraRoi * 0.5f);
            if (_p4.Equals(Vector2.zero)) _p4 = new Vector2(-cameraRoi * 0.5f, -cameraRoi * 0.5f);
            // 判断点p是否在p1p2p3p4的正方形内
            bool isPointIn = GetCross(_p1, _p2, p) * GetCross(_p3, _p4, p) >= 0 &&
                             GetCross(_p2, _p3, p) * GetCross(_p4, _p1, p) >= 0;
            return isPointIn;
        }

        private float GetCross(Vector2 p1, Vector2 p2, Vector2 p)
        {
            return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
        }
    }
}
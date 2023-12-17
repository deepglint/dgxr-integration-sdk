using System.Collections;
using System.Collections.Generic;
using Moat.Model;
using UnityEngine;

namespace Moat
{
    public class ROITools : MSingleton<ROITools>
    {
        private float[][] _interactionAreaPosition = new float[][]
        {
            new float[] { 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f },
        };

        public float GetTestPosX(int area)
        {
            if (area > 0 && area <= 5)
            {
                return (_interactionAreaPosition[area - 1][0] + _interactionAreaPosition[area - 1][1]) / 2;
            }

            return 10f;
        }

        public void UpdateRoi()
        {
            DisplayData.ReadConfig();
            float[][] interactionArea = DisplayData.configDisplay.interactionArea;
            for (int i = 0; i < interactionArea.Length; i++)
            {
                _interactionAreaPosition[i] = new float[]
                    { (float)interactionArea[i][0], (float)interactionArea[i][1], (float)interactionArea[i][2] };
            }
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

        private Vector2 p1, p2, p3, p4;

        // roi边界检测
        public bool CheckBoundary(Vector2 p)
        {
            float cameraRoi = DisplayData.configDisplay.resolution.realResolution;

            if (p1.Equals(Vector2.zero)) p1 = new Vector2(-cameraRoi * 0.5f, cameraRoi * 0.5f);
            if (p2.Equals(Vector2.zero)) p2 = new Vector2(cameraRoi * 0.5f, cameraRoi * 0.5f);
            if (p3.Equals(Vector2.zero)) p3 = new Vector2(cameraRoi * 0.5f, -cameraRoi * 0.5f);
            if (p4.Equals(Vector2.zero)) p4 = new Vector2(-cameraRoi * 0.5f, -cameraRoi * 0.5f);
            // 判断点p是否在p1p2p3p4的正方形内
            bool isPointIn = GetCross(p1, p2, p) * GetCross(p3, p4, p) >= 0 &&
                             GetCross(p2, p3, p) * GetCross(p4, p1, p) >= 0;
            return isPointIn;
        }

        private float GetCross(Vector2 p1, Vector2 p2, Vector2 p)
        {
            return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
        }
    }
}
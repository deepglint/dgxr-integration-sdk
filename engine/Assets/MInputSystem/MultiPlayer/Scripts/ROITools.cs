using System.Collections;
using System.Collections.Generic;
using Moat.Model;
using UnityEngine;

public class ROITools: MSingleton<ROITools>
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
            _interactionAreaPosition[i] = new float[] { (float)interactionArea[i][0], (float)interactionArea[i][1], (float)interactionArea[i][2] };
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
        if (leftFootPos.x >= areaPosition[0] && leftFootPos.x < areaPosition[1] && leftFootPos.y >= -areaPosition[2] && leftFootPos.y <= areaPosition[2])
        {
            return true;
        }
        return false;
    }
}

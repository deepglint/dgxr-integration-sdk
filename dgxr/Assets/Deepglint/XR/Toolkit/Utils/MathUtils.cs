using UnityEngine;
namespace Deepglint.XR.Toolkit.Utils
{
    public static class MathUtils
    {
        // 计算点与圆相交
        public static bool PointCircleIntersection(Vector2 circleCenter, Vector2 point, float circleRadius)
        {
            float distance = Vector2.Distance(circleCenter, point);
            return distance < circleRadius;
        }

        // 计算两个圆相交
        public static bool CircleCircleIntersection(Vector2 circle1Center, float circle1Radius, Vector2 circle2Center, float circle2Radius)
        {
            float distance = Vector2.Distance(circle1Center, circle2Center);
            return distance <= circle1Radius + circle2Radius;
        }

        // 计算圆与矩形相交
        public static bool CircleRectangleIntersection(Vector2 circleCenter, float circleRadius, Rect rectangle)
        {
            Vector2 closestPoint = new Vector2(
                Mathf.Clamp(circleCenter.x, rectangle.xMin, rectangle.xMax),
                Mathf.Clamp(circleCenter.y, rectangle.yMin, rectangle.yMax)
            );

            float distance = Vector2.Distance(circleCenter, closestPoint);
            return distance <= circleRadius;
        } 
        
        // 计算点与矩形相交
        public static bool PointRectangleIntersection(Vector2 point, Vector2 rectangle, Vector2 sizeDelta)
        {
            // bool isLeftEntered = MathUtils.PointRectangleIntersection(leftPos, groundSelectorRect.anchoredPosition, groundSelectorRect.sizeDelta);
            Rect rect = new Rect(rectangle - sizeDelta / 2f, sizeDelta);
            return rect.Contains(point);
        }  

    }
}
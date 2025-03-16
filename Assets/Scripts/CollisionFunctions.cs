using UnityEngine;
using System.Collections.Generic;
// esta clase contiene las funciones con las formulas de colisones con todas las combinaciones posibles.
public class CollisionFunctions : MonoBehaviour
{
    public static bool PointToAABB(Vector2 point, Vector2 min, Vector2 max)
    {
        return (point.x >= min.x && point.x <= max.x &&
                point.y >= min.y && point.y <= max.y);
    }
    public static bool PointToCircle(Vector2 point, Vector2 center, float radius)
    {
        return (point - center).sqrMagnitude <= radius * radius;
    }
    public static bool PointToOBB(Vector2 point, Transform boxTransform)
    {
        Vector2 localPoint = boxTransform.InverseTransformPoint(point);
        Vector2 halfSize = boxTransform.localScale / 2;
        return Mathf.Abs(localPoint.x) <= halfSize.x && Mathf.Abs(localPoint.y) <= halfSize.y;
    }
    public static bool AABBToAABB(Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB)
    {
        return (minA.x < maxB.x && maxA.x > minB.x &&
                minA.y < maxB.y && maxA.y > minB.y);
    }
    public static bool CircleToAABB(Vector2 circleCenter, float radius, Vector2 min, Vector2 max)
    {
        float closestX = Mathf.Clamp(circleCenter.x, min.x, max.x);
        float closestY = Mathf.Clamp(circleCenter.y, min.y, max.y);
        Vector2 closestPoint = new Vector2(closestX, closestY);
        return (circleCenter - closestPoint).sqrMagnitude <= radius * radius;
    }
    public static bool CircleToCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
    {
        float radiusSum = radiusA + radiusB;
        return (centerA - centerB).sqrMagnitude <= radiusSum * radiusSum;
    }
    public static bool CircleToOBB(Vector2 circleCenter, float radius, Transform boxTransform)
    {
        Vector2 localPoint = boxTransform.InverseTransformPoint(circleCenter);
        Vector2 halfSize = boxTransform.localScale / 2;
        float closestX = Mathf.Clamp(localPoint.x, -halfSize.x, halfSize.x);
        float closestY = Mathf.Clamp(localPoint.y, -halfSize.y, halfSize.y);
        Vector2 closestPoint = new Vector2(closestX, closestY);
        return (closestPoint - localPoint).sqrMagnitude <= radius * radius;
    }
    public static bool OBBToAABB(Transform obb, Vector2 aabbMin, Vector2 aabbMax)
    {
        Vector2[] axes = new Vector2[2]
        {
        obb.right.normalized,
        obb.up.normalized
        };

        Vector2 obbHalfSize = obb.localScale / 2;

        Vector2 obbCornersWorldSpace = obb.position;
        Vector2[] obbCorners = new Vector2[4]
        {
        obbCornersWorldSpace + axes[0] * obbHalfSize.x + axes[1] * obbHalfSize.y,
        obbCornersWorldSpace - axes[0] * obbHalfSize.x + axes[1] * obbHalfSize.y,
        obbCornersWorldSpace + axes[0] * obbHalfSize.x - axes[1] * obbHalfSize.y,
        obbCornersWorldSpace - axes[0] * obbHalfSize.x - axes[1] * obbHalfSize.y
        };

        foreach (Vector2 corner in obbCorners)
        {
            if (PointToAABB(corner, aabbMin, aabbMax))
                return true;
        }

        return false;
    }
    public static bool OBBToOBB(Transform boxA, Transform boxB)
    {
        Vector2[] axes = new Vector2[4]
        {
            boxA.right,
            boxA.up,
            boxB.right,
            boxB.up
        };        
        return true;
    }    
}

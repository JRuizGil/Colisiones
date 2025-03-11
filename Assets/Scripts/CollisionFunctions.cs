using UnityEngine;

public static class CollisionFunctions
{
    //  Punto vs AABB
    public static bool PointToAABB(Vector2 point, Vector2 boxPos, Vector2 boxSize)
    {
        return point.x >= boxPos.x - boxSize.x / 2 &&
               point.x <= boxPos.x + boxSize.x / 2 &&
               point.y >= boxPos.y - boxSize.y / 2 &&
               point.y <= boxPos.y + boxSize.y / 2;
        //if (boxPos.x + boxSize.x / 2 >= point.x && point.x >= boxPos.x + boxSize.x / 2)
        //{
        //    if(boxPos.y + boxSize.y / 2 >= point.y && point.y >= boxPos.y + boxSize.y / 2)
        //    {
        //        return true;
        //    }
        //}
    }
    //  Punto vs Círculo
    public static bool PointToCircle(Vector2 point, Vector2 circlePos, float radius)
    {
        return (point - circlePos).sqrMagnitude <= radius * radius;
    }

    //  Punto vs OBB (Separating Axis Theorem)
    public static bool PointToOBB(Vector2 point, Vector2 boxPos, Vector2 boxSize, float angle)
    {
        Vector2 localPoint = Quaternion.Euler(0, 0, -angle) * (point - boxPos);
        return Mathf.Abs(localPoint.x) <= boxSize.x / 2 && Mathf.Abs(localPoint.y) <= boxSize.y / 2;
    }

    //  AABB vs AABB
    public static bool AABBToAABB(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
    {
        return !(pos1.x + size1.x / 2 < pos2.x - size2.x / 2 ||
                 pos1.x - size1.x / 2 > pos2.x + size2.x / 2 ||
                 pos1.y + size1.y / 2 < pos2.y - size2.y / 2 ||
                 pos1.y - size1.y / 2 > pos2.y + size2.y / 2);
    }

    //  Circle vs AABB
    public static bool CircleToAABB(Vector2 circlePos, float radius, Vector2 boxPos, Vector2 boxSize)
    {
        Vector2 closestPoint = new Vector2(
            Mathf.Clamp(circlePos.x, boxPos.x - boxSize.x / 2, boxPos.x + boxSize.x / 2),
            Mathf.Clamp(circlePos.y, boxPos.y - boxSize.y / 2, boxPos.y + boxSize.y / 2)
        );
        return (circlePos - closestPoint).sqrMagnitude <= radius * radius;
    }

    //  Circle vs Circle
    public static bool CircleToCircle(Vector2 pos1, float radius1, Vector2 pos2, float radius2)
    {
        float distance = (pos1 - pos2).sqrMagnitude;
        float radiusSum = radius1 + radius2;
        return distance <= radiusSum * radiusSum;
    }

    //  Circle vs OBB (Usa PointToOBB con el círculo)
    public static bool CircleToOBB(Vector2 circlePos, float radius, Vector2 boxPos, Vector2 boxSize, float angle)
    {
        Vector2 closestPoint = Quaternion.Euler(0, 0, -angle) * (circlePos - boxPos);
        closestPoint.x = Mathf.Clamp(closestPoint.x, -boxSize.x / 2, boxSize.x / 2);
        closestPoint.y = Mathf.Clamp(closestPoint.y, -boxSize.y / 2, boxSize.y / 2);
        return (closestPoint - circlePos).sqrMagnitude <= radius * radius;
    }
}

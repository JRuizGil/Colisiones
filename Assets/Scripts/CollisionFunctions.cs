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
    public static bool AABBToAABBResolution(Vector2 center1, Vector2 scale1, Vector2 center2, Vector2 scale2, out Contacto outContact)
    {
        Vector2 halfSize1 = scale1 / 2;
        Vector2 halfSize2 = scale2 / 2;

        Vector2 minA = center1 - halfSize1;
        Vector2 maxA = center1 + halfSize1;
        Vector2 minB = center2 - halfSize2;
        Vector2 maxB = center2 + halfSize2;

        if (!AABBToAABB(minA, maxA, minB, maxB))
        {
            outContact = new Contacto(Vector2.zero, Vector2.zero, 0);
            return false;
        }

        Vector2 direction = center2 - center1;
        float magX = (halfSize1.x + halfSize2.x) - Mathf.Abs(direction.x);
        float magY = (halfSize1.y + halfSize2.y) - Mathf.Abs(direction.y);

        Vector2 contactDirection;
        if (magX < magY)
        {
            contactDirection = new Vector2(Mathf.Sign(direction.x), 0);
        }
        else
        {
            contactDirection = new Vector2(0, Mathf.Sign(direction.y));
        }

        if (magX == magY)
        {
            contactDirection = -contactDirection;
        }

        float penetrationDepth = Mathf.Min(magX, magY);
        Vector2 contactPoint = Vector2.zero;

        List<Vector2> vertices = new List<Vector2>
    {
        new Vector2(minA.x, minA.y), new Vector2(minA.x, maxA.y),
        new Vector2(maxA.x, minA.y), new Vector2(maxA.x, maxA.y),
        new Vector2(minB.x, minB.y), new Vector2(minB.x, maxB.y),
        new Vector2(maxB.x, minB.y), new Vector2(maxB.x, maxB.y)
    };

        foreach (var vertex in vertices)
        {
            if (PointToAABB(vertex, minB, maxB))
            {
                contactPoint = vertex;
                break;
            }
            if (PointToAABB(vertex, minA, maxA))
            {
                contactPoint = vertex;
                break;
            }
        }

        outContact = new Contacto(contactPoint, contactDirection, penetrationDepth);
        return true;
    }

    public static bool CircleToAABB(Vector2 circleCenter, float radius, Vector2 min, Vector2 max)
    {
        float closestX = Mathf.Clamp(circleCenter.x, min.x, max.x);
        float closestY = Mathf.Clamp(circleCenter.y, min.y, max.y);
        Vector2 closestPoint = new Vector2(closestX, closestY);
        return (circleCenter - closestPoint).sqrMagnitude <= radius * radius;
    }
    public static bool CircleToAABBResolution(Vector2 centerCircle, Vector2 scaleCircle, Vector2 centerAABB, Vector2 scaleAABB, out Contacto outContact)
    {
        float radius = scaleCircle.x / 2;
        Vector2 halfSize = scaleAABB / 2;
        Vector2 minAABB = centerAABB - halfSize;
        Vector2 maxAABB = centerAABB + halfSize;

        // Encuentra el punto más cercano dentro del AABB
        float closestX = Mathf.Clamp(centerCircle.x, minAABB.x, maxAABB.x);
        float closestY = Mathf.Clamp(centerCircle.y, minAABB.y, maxAABB.y);
        Vector2 closestPoint = new Vector2(closestX, closestY);

        // Dirección desde el punto más cercano hasta el centro del círculo
        Vector2 direction = centerCircle - closestPoint;
        float distance = direction.magnitude;

        //  Variables declaradas correctamente
        Vector2 contactDirection = Vector2.zero;
        Vector2 contactPoint = Vector2.zero;
        float penetrationDepth = 0f;

        if (distance < radius) // Si hay colisión
        {
            if (distance == 0)
            {
                // Si el círculo está completamente dentro del AABB, empujarlo en la dirección más corta
                Vector2 pushDirection = (centerCircle - centerAABB).normalized;
                if (pushDirection == Vector2.zero) pushDirection = Vector2.up; // Evita NaN

                // Encuentra la menor penetración en X o Y y empuja en esa dirección
                float magX = halfSize.x - Mathf.Abs(centerCircle.x - centerAABB.x);
                float magY = halfSize.y - Mathf.Abs(centerCircle.y - centerAABB.y);
                penetrationDepth = Mathf.Min(magX, magY) + radius;

                contactDirection = pushDirection;
            }
            else
            {
                // Normalizar dirección y calcular la penetración
                contactDirection = direction.normalized;
                penetrationDepth = radius - distance;
            }

            contactPoint = closestPoint;
            outContact = new Contacto(contactPoint, contactDirection, penetrationDepth);
            return true;
        }

        outContact = new Contacto(Vector2.zero, Vector2.zero, 0);
        return false;
    }





    public static bool CircleToCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
    {
        float radiusSum = radiusA + radiusB;
        return (centerA - centerB).sqrMagnitude <= radiusSum * radiusSum;
    }
    public static bool CircleToCircleResolution(Vector2 center1, Vector2 scale1, Vector2 center2, Vector2 scale2, out Contacto outContact)
    {
        float radius1 = scale1.x / 2; // Se asume que el círculo usa escala para el diámetro
        float radius2 = scale2.x / 2;

        Vector2 direction = center2 - center1; // Dirección del centro de un círculo al otro
        float distance = direction.magnitude; // Distancia entre los círculos
        float radiusSum = radius1 + radius2; // Suma de los radios

        if (distance < radiusSum) // Si hay colisión
        {
            Vector2 contactDirection = direction.normalized; // Normalizamos la dirección del contacto
            float penetrationDepth = radiusSum - distance; // Magnitud de contacto (penetración)

            // Calculamos el punto de contacto
            Vector2 contactPoint = center1 + contactDirection * radius1;

            // Guardamos los datos en la estructura Contacto
            outContact = new Contacto(contactPoint, contactDirection, penetrationDepth);
            return true;
        }

        outContact = new Contacto(Vector2.zero, Vector2.zero, 0);
        return false;
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
    public static bool CircleToOBBResolution(Vector2 centerCircle, Vector2 scaleCircle, Transform OBB, out Contacto outContact)
    {
        Vector3 originalScale = OBB.localScale;
        OBB.localScale = Vector3.one; // Normalizamos la escala para la detección de colisión

        Vector2 localCircleCenter = OBB.InverseTransformPoint(centerCircle);
        Vector2 localCircleScale = scaleCircle / originalScale;
        Vector2 obbScale = Vector2.one;

        bool collision = CircleToAABBResolution(localCircleCenter, localCircleScale, Vector2.zero, obbScale, out outContact);

        if (collision)
        {
            outContact.mPuntoContacto = OBB.TransformPoint(outContact.mPuntoContacto);
            outContact.mDirecciónContacto = (OBB.TransformDirection(outContact.mDirecciónContacto)).normalized;
        }

        OBB.localScale = originalScale; // Restaurar escala original
        return collision;
    }
    public static bool AABBToOBB(Transform obb, Vector2 aabbMin, Vector2 aabbMax)
    {
        Vector2[] axes = new Vector2[2]
        {
        obb.right.normalized,
        obb.up.normalized
        };

        Vector2 obbHalfSize = obb.localScale / 2;
        Vector2 obbCenter = obb.position;

        Vector2[] obbCorners = new Vector2[4]
        {
        obbCenter + axes[0] * obbHalfSize.x + axes[1] * obbHalfSize.y,
        obbCenter - axes[0] * obbHalfSize.x + axes[1] * obbHalfSize.y,
        obbCenter + axes[0] * obbHalfSize.x - axes[1] * obbHalfSize.y,
        obbCenter - axes[0] * obbHalfSize.x - axes[1] * obbHalfSize.y
        };

        // Proyectar el AABB en cada eje del OBB y verificar separación
        foreach (Vector2 axis in axes)
        {
            float minAABB = Mathf.Min(Vector2.Dot(aabbMin, axis), Vector2.Dot(aabbMax, axis));
            float maxAABB = Mathf.Max(Vector2.Dot(aabbMin, axis), Vector2.Dot(aabbMax, axis));

            float minOBB = float.MaxValue, maxOBB = float.MinValue;
            foreach (Vector2 corner in obbCorners)
            {
                float projection = Vector2.Dot(corner, axis);
                minOBB = Mathf.Min(minOBB, projection);
                maxOBB = Mathf.Max(maxOBB, projection);
            }

            if (maxAABB < minOBB || maxOBB < minAABB) return false;
        }

        return true;
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

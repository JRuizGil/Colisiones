using UnityEngine;
using System.Collections.Generic;
// esta clase contiene las funciones con las formulas de colisones con todas las combinaciones posibles.
public class CollisionFunctions : MonoBehaviour
{
    //funciones de las colisiones
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
    public static bool AABBToOBB(Transform obb, Vector2 aabbMin, Vector2 aabbMax)
    {        
        Vector2 obbCenter = obb.position;
        Vector2 obbHalfSize = obb.localScale / 2;
                
        Vector2[] axes = new Vector2[2]
        {
        obb.right.normalized, 
        obb.up.normalized    
        };
                
        Vector2 aabbCenter = (aabbMin + aabbMax) / 2;
        Vector2 aabbHalfSize = (aabbMax - aabbMin) / 2;
                
        Vector2[] obbCorners = new Vector2[4]
        {
        obbCenter + axes[0] * obbHalfSize.x + axes[1] * obbHalfSize.y,
        obbCenter - axes[0] * obbHalfSize.x + axes[1] * obbHalfSize.y,
        obbCenter + axes[0] * obbHalfSize.x - axes[1] * obbHalfSize.y,
        obbCenter - axes[0] * obbHalfSize.x - axes[1] * obbHalfSize.y
        };
                
        foreach (Vector2 axis in axes)
        {            
            float minAABB = Vector2.Dot(aabbCenter, axis) - (Mathf.Abs(aabbHalfSize.x * Vector2.Dot(Vector2.right, axis)) +
                                                             Mathf.Abs(aabbHalfSize.y * Vector2.Dot(Vector2.up, axis)));
            float maxAABB = Vector2.Dot(aabbCenter, axis) + (Mathf.Abs(aabbHalfSize.x * Vector2.Dot(Vector2.right, axis)) +
                                                             Mathf.Abs(aabbHalfSize.y * Vector2.Dot(Vector2.up, axis)));
                        
            float minOBB = float.MaxValue, maxOBB = float.MinValue;
            foreach (Vector2 corner in obbCorners)
            {
                float projection = Vector2.Dot(corner, axis);
                minOBB = Mathf.Min(minOBB, projection);
                maxOBB = Mathf.Max(maxOBB, projection);
            }                        
            if (maxAABB < minOBB || maxOBB < minAABB)
            {
                return false;
            }
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
    //resoluciones de las colisiones
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
    public static bool CircleToAABBResolution(Vector2 centerCircle, Vector2 scaleCircle, Vector2 centerAABB, Vector2 scaleAABB, out Contacto outContact)
    {        
        float radius = scaleCircle.x / 2;
        
        Vector2 halfSize = scaleAABB / 2;
        
        Vector2 minAABB = centerAABB - halfSize;
        Vector2 maxAABB = centerAABB + halfSize;
                
        float closestX = Mathf.Clamp(centerCircle.x, minAABB.x, maxAABB.x);
        float closestY = Mathf.Clamp(centerCircle.y, minAABB.y, maxAABB.y);
        Vector2 closestPoint = new Vector2(closestX, closestY);
                
        Vector2 direction = centerCircle - closestPoint;
        float distance = direction.magnitude;

        Vector2 contactDirection = Vector2.zero;
        Vector2 contactPoint = Vector2.zero;
        float penetrationDepth = 0f;
                
        if (distance < radius)
        {
            if (distance == 0) 
            {                
                float magX = halfSize.x - Mathf.Abs(centerCircle.x - centerAABB.x);
                float magY = halfSize.y - Mathf.Abs(centerCircle.y - centerAABB.y);                
                if (magX < magY)
                {
                    contactDirection = (centerCircle.x > centerAABB.x) ? Vector2.right : Vector2.left;
                    penetrationDepth = magX + radius;
                }
                else
                {
                    contactDirection = (centerCircle.y > centerAABB.y) ? Vector2.up : Vector2.down;
                    penetrationDepth = magY + radius;
                }
                contactPoint = centerCircle; 
            }
            else 
            {
                contactDirection = direction.normalized;
                penetrationDepth = radius - distance;
                contactPoint = closestPoint;
            }            
            outContact = new Contacto(contactPoint, contactDirection, penetrationDepth);
            return true;
        }        
        outContact = new Contacto(Vector2.zero, Vector2.zero, 0);
        return false;
    }
    public static bool CircleToCircleResolution(Vector2 center1, Vector2 scale1, Vector2 center2, Vector2 scale2, out Contacto outContact)
    {
        float radius1 = scale1.x / 2; 
        float radius2 = scale2.x / 2;

        Vector2 direction = center2 - center1; 
        float distance = direction.magnitude; 
        float radiusSum = radius1 + radius2; 

        if (distance < radiusSum) 
        {
            Vector2 contactDirection = direction.normalized; 
            float penetrationDepth = radiusSum - distance; 
            
            Vector2 contactPoint = center1 + contactDirection * radius1;
            
            outContact = new Contacto(contactPoint, contactDirection, penetrationDepth);
            return true;
        }
        outContact = new Contacto(Vector2.zero, Vector2.zero, 0);
        return false;
    }
    public static bool CircleToOBBResolution(Vector2 centerCircle, Vector2 scaleCircle, Transform OBB, out Contacto outContact)
    {
        Vector3 originalScale = OBB.localScale;
        OBB.localScale = Vector3.one; 

        Vector2 localCircleCenter = OBB.InverseTransformPoint(centerCircle);
        Vector2 localCircleScale = scaleCircle / originalScale;
        Vector2 obbScale = Vector2.one;

        bool collision = CircleToAABBResolution(localCircleCenter, localCircleScale, Vector2.zero, obbScale, out outContact);

        if (collision)
        {
            outContact.mPuntoContacto = OBB.TransformPoint(outContact.mPuntoContacto);
            outContact.mDirecciónContacto = (OBB.TransformDirection(outContact.mDirecciónContacto)).normalized;
        }

        OBB.localScale = originalScale; 
        return collision;
    }
    public static bool OBBToAABBResolution(Transform obb, Vector2 aabbCenter, Vector2 aabbSize, out Contacto contacto)
    {
        Vector2 aabbMin = aabbCenter - aabbSize / 2;
        Vector2 aabbMax = aabbCenter + aabbSize / 2;

        if (!AABBToOBB(obb, aabbMin, aabbMax))
        {
            contacto = new Contacto(Vector2.zero, Vector2.zero, 0);
            return false;
        }

        Vector2 closestPoint = obb.InverseTransformPoint(aabbCenter);
        Vector2 halfSize = obb.localScale / 2;
        Vector2 clampedPoint = Vector2.Max(-halfSize, Vector2.Min(closestPoint, halfSize));

        Vector2 direction = closestPoint - clampedPoint;
        float distance = direction.magnitude;
        float penetration = halfSize.magnitude - distance;

        if (penetration > 0)
        {
            Vector2 normal = obb.TransformDirection(direction.normalized);
            contacto = new Contacto(normal * penetration, normal, penetration);
            return true;
        }

        contacto = new Contacto(Vector2.zero, Vector2.zero, 0);
        return false;
    }
    public static bool OBBToOBBResolution(Transform OBB1, Transform OBB2, out Contacto outContact)
    {        
        Vector2 direction = OBB2.position - OBB1.position;
        
        Vector2[] axes =
        {
            new Vector2(Mathf.Cos(OBB1.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(OBB1.rotation.eulerAngles.z * Mathf.Deg2Rad)),
            new Vector2(-Mathf.Sin(OBB1.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Cos(OBB1.rotation.eulerAngles.z * Mathf.Deg2Rad)),
            new Vector2(Mathf.Cos(OBB2.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(OBB2.rotation.eulerAngles.z * Mathf.Deg2Rad)),
            new Vector2(-Mathf.Sin(OBB2.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Cos(OBB2.rotation.eulerAngles.z * Mathf.Deg2Rad))
        };
        
        float minOverlap = float.MaxValue;
        Vector2 minAxis = Vector2.zero;
        foreach (Vector2 axis in axes)
        {            
            float projDir = Mathf.Abs(Vector2.Dot(direction, axis));
                        
            float projA = GetOBBProjection(OBB1, axis);
            float projB = GetOBBProjection(OBB2, axis);
                        
            float overlap = projA + projB - projDir;
                        
            if (overlap < 0)
            {
                outContact = new Contacto(Vector2.zero, Vector2.zero, 0);
                return false;
            }
                        
            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                minAxis = axis;
            }
        }        
        if (Vector2.Dot(direction, minAxis) < 0)
        {
            minAxis = -minAxis;
        }        
        Vector2 contactPoint = GetContactPoint(OBB1, OBB2);
                
        outContact = new Contacto(minAxis * minOverlap, minAxis, minOverlap);

        return true;
    }

    // Calcula la proyección de un OBB sobre un eje
    private static float GetOBBProjection(Transform OBB, Vector2 axis)
    {
        Vector2 right = new Vector2(OBB.right.x, OBB.right.y);
        Vector2 up = new Vector2(OBB.up.x, OBB.up.y);
        Vector2 halfSize = OBB.localScale / 2;

        // Proyección de los vectores del OBB sobre el eje
        float projRight = Mathf.Abs(Vector2.Dot(right, axis)) * halfSize.x;
        float projUp = Mathf.Abs(Vector2.Dot(up, axis)) * halfSize.y;

        return projRight + projUp;
    }

    // Encuentra el punto de contacto entre los OBBs
    private static Vector2 GetContactPoint(Transform OBB1, Transform OBB2)
    {
        // Calcula las esquinas de ambos OBBs
        Vector2[] corners1 = GetOBBCorners(OBB1);
        Vector2[] corners2 = GetOBBCorners(OBB2);

        // Comprueba las esquinas del primer OBB si están dentro del otro OBB
        foreach (Vector2 corner in corners1)
        {
            if (PointToOBB(corner, OBB2))
                return corner;
        }

        // Si no, comprueba las esquinas del segundo OBB
        foreach (Vector2 corner in corners2)
        {
            if (PointToOBB(corner, OBB1))
                return corner;
        }

        // Si no hay contacto directo, devuelve el punto medio de los OBBs
        return (OBB1.position + OBB2.position) / 2;
    }

    // Calcula las esquinas de un OBB
    private static Vector2[] GetOBBCorners(Transform OBB)
    {
        Vector2 center = OBB.position;
        Vector2 right = new Vector2(OBB.right.x, OBB.right.y);
        Vector2 up = new Vector2(OBB.up.x, OBB.up.y);
        Vector2 halfSize = OBB.localScale / 2;

        // Devuelve las cuatro esquinas del OBB
        return new Vector2[]
        {
        center + halfSize.x * right + halfSize.y * up,
        center + halfSize.x * right - halfSize.y * up,
        center - halfSize.x * right + halfSize.y * up,
        center - halfSize.x * right - halfSize.y * up
        };
    }

}

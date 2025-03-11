using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance;
    private List<CustomCollider> colliders = new List<CustomCollider>();

    void Awake()
    {
        Instance = this;
        // Registrar todos los objetos ya presentes en la escena usando el nuevo método
        CustomCollider[] existingColliders = FindObjectsByType<CustomCollider>(FindObjectsSortMode.None);
        foreach (var col in existingColliders)
        {
            Register(col);
        }
    }

    public void Register(CustomCollider col)
    {
        if (!colliders.Contains(col))
        {
            colliders.Add(col);
        }
    }

    void Update()
    {
        foreach (var col1 in colliders)
        {
            foreach (var col2 in colliders)
            {
                if (col1 == col2) continue;

                bool collision = false;
                // Combinaciones de colisión
                if (col1.colliderType == CustomCollider.ColliderType.Circle && col2.colliderType == CustomCollider.ColliderType.AABB)
                    collision = CollisionFunctions.CircleToAABB(col1.transform.position, col1.Radius, col2.transform.position, col2.Size);
                if (col1.colliderType == CustomCollider.ColliderType.Circle && col2.colliderType == CustomCollider.ColliderType.Circle)
                    collision = CollisionFunctions.CircleToCircle(col1.transform.position, col1.Radius, col2.transform.position, col2.Radius);
                if (col1.colliderType == CustomCollider.ColliderType.AABB && col2.colliderType == CustomCollider.ColliderType.AABB)
                    collision = CollisionFunctions.AABBToAABB(col1.transform.position, col1.Size, col2.transform.position, col2.Size);
                if (col1.colliderType == CustomCollider.ColliderType.Circle && col2.colliderType == CustomCollider.ColliderType.OBB)
                    collision = CollisionFunctions.CircleToOBB(col1.transform.position, col1.Radius, col2.transform.position, col2.Size, col2.transform.eulerAngles.z);
                if (col1.colliderType == CustomCollider.ColliderType.OBB && col2.colliderType == CustomCollider.ColliderType.AABB)
                    collision = CollisionFunctions.PointToOBB(col2.transform.position, col1.transform.position, col1.Size, col1.transform.eulerAngles.z);
                if (col1.colliderType == CustomCollider.ColliderType.OBB && col2.colliderType == CustomCollider.ColliderType.Circle)
                    collision = CollisionFunctions.CircleToOBB(col2.transform.position, col2.Radius, col1.transform.position, col1.Size, col1.transform.eulerAngles.z);
                if (col1.colliderType == CustomCollider.ColliderType.OBB && col2.colliderType == CustomCollider.ColliderType.OBB)
                    collision = CollisionFunctions.PointToOBB(col1.transform.position, col2.transform.position, col2.Size, col2.transform.eulerAngles.z) ||
                                CollisionFunctions.PointToOBB(col2.transform.position, col1.transform.position, col1.Size, col1.transform.eulerAngles.z);

                if (collision)
                {
                    col1.SetColor(Color.red);
                    col2.SetColor(Color.green);
                }
                else if (!col1.IsDragging && !col2.IsDragging)
                {
                    col1.SetColor(Color.white);
                    col2.SetColor(Color.white);
                }
            }
        }
    }
}
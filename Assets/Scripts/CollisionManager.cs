using System.Collections.Generic;
using UnityEngine;
//clase para checkear cada frame las colisiones que ocurren en toda la escena.
public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance;
    private List<CustomCollider> colliders = new List<CustomCollider>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    // para registrar objetos de escena
    public void RegisterCollider(CustomCollider collider)
    {
        colliders.Add(collider);
    }    
    // recorrido de objetos y cambio de color cuando colisionan
    void Update()
    {
        HashSet<CustomCollider> collidingObjects = new HashSet<CustomCollider>();

        foreach (var colA in colliders)
        {
            foreach (var colB in colliders)
            {
                if (colA == colB) continue;
                if (CheckCollision(colA, colB))
                {
                    colA.GetComponent<SpriteRenderer>().color = Color.red;
                    colB.GetComponent<SpriteRenderer>().color = Color.green;
                    collidingObjects.Add(colA);
                    collidingObjects.Add(colB);
                }
            }
        }
        foreach (var col in colliders)
        {
            if (!collidingObjects.Contains(col))
            {
                col.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
    //funcion que lee combinaciones de colisiones posibles sucediendo en encena
    private bool CheckCollision(CustomCollider colA, CustomCollider colB)
    {
        if (colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.AABB)
            return CollisionFunctions.AABBToAABB(colA.transform.position - colA.transform.localScale / 2, colA.transform.position + colA.transform.localScale / 2, colB.transform.position - colB.transform.localScale / 2, colB.transform.position + colB.transform.localScale / 2);
        if (colA.colliderType == CustomCollider.ColliderType.Circle && colB.colliderType == CustomCollider.ColliderType.AABB || colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.Circle)
            return CollisionFunctions.CircleToAABB(colA.transform.position, colA.transform.localScale.x / 2, colB.transform.position - colB.transform.localScale / 2, colB.transform.position + colB.transform.localScale / 2);
        if (colA.colliderType == CustomCollider.ColliderType.Circle && colB.colliderType == CustomCollider.ColliderType.Circle)
            return CollisionFunctions.CircleToCircle(colA.transform.position, colA.transform.localScale.x / 2, colB.transform.position, colB.transform.localScale.x / 2);
        if (colA.colliderType == CustomCollider.ColliderType.Circle && colB.colliderType == CustomCollider.ColliderType.OBB || colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.Circle)
            return CollisionFunctions.CircleToOBB(colA.transform.position, colA.transform.localScale.x / 2, colB.transform);
        if (colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.OBB || colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.AABB)
            return CollisionFunctions.CircleToAABB(colA.transform.position, colA.transform.localScale.x / 2, colB.transform.position - colB.transform.localScale / 2, colB.transform.position + colB.transform.localScale / 2);
        if (colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.OBB)
            return CollisionFunctions.PointToOBB(colA.transform.position, colB.transform);
        if (colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.OBB)
            return CollisionFunctions.OBBToAABB(colB.transform, colA.transform.position - colA.transform.localScale / 2, colA.transform.position + colA.transform.localScale / 2);
        if (colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.AABB)
            return CollisionFunctions.OBBToAABB(colA.transform, colB.transform.position - colB.transform.localScale / 2, colB.transform.position + colB.transform.localScale / 2);
        return false;
    }
}


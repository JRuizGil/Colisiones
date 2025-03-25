using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance;
    private List<CustomCollider> colliders = new List<CustomCollider>();
    public bool applyResolution = true; // Activa o desactiva la resolución de colisiones

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterCollider(CustomCollider collider)
    {
        colliders.Add(collider);
    }

    void Update()
    {
        HashSet<CustomCollider> collidingObjects = new HashSet<CustomCollider>();

        foreach (var colA in colliders)
        {
            foreach (var colB in colliders)
            {
                if (colA == colB) continue;

                if (ResolveCollision(colA, colB, applyResolution))
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
    private bool ResolveCollision(CustomCollider colA, CustomCollider colB, bool applyResolution)
    {
        Contacto contact = new Contacto(Vector2.zero, Vector2.zero, 0);
        bool collisionDetected = false;

        if (colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.AABB)
        {
            collisionDetected = CollisionFunctions.AABBToAABBResolution(
                colA.transform.position, colA.transform.localScale,
                colB.transform.position, colB.transform.localScale, out contact);
        }
        else if (colA.colliderType == CustomCollider.ColliderType.Circle && colB.colliderType == CustomCollider.ColliderType.AABB)
        {
            collisionDetected = CollisionFunctions.CircleToAABBResolution(
                colA.transform.position, new Vector2(colA.transform.localScale.x, colA.transform.localScale.x),
                colB.transform.position, colB.transform.localScale, out contact);
        }
        else if (colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.Circle)
        {
            collisionDetected = CollisionFunctions.CircleToAABBResolution(
                colB.transform.position, new Vector2(colB.transform.localScale.x, colB.transform.localScale.x),
                colA.transform.position, colA.transform.localScale, out contact);

            contact.mDirecciónContacto = -contact.mDirecciónContacto; // Invertir dirección
        }
        else if (colA.colliderType == CustomCollider.ColliderType.Circle && colB.colliderType == CustomCollider.ColliderType.Circle)
        {
            collisionDetected = CollisionFunctions.CircleToCircleResolution(
                colA.transform.position, new Vector2(colA.transform.localScale.x, colA.transform.localScale.x),
                colB.transform.position, new Vector2(colB.transform.localScale.x, colB.transform.localScale.x), out contact);
        }
        else if (colA.colliderType == CustomCollider.ColliderType.Circle && colB.colliderType == CustomCollider.ColliderType.OBB)
        {
            collisionDetected = CollisionFunctions.CircleToOBBResolution(
                colA.transform.position, new Vector2(colA.transform.localScale.x, colA.transform.localScale.x),
                colB.transform, out contact);
        }
        else if (colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.Circle)
        {
            collisionDetected = CollisionFunctions.CircleToOBBResolution(
                colB.transform.position, new Vector2(colB.transform.localScale.x, colB.transform.localScale.x),
                colA.transform, out contact);

            contact.mDirecciónContacto = -contact.mDirecciónContacto;
        }

        //  Corrección en la resolución de penetración
        if (collisionDetected && applyResolution)
        {
            if (contact.mMagnitudContacto > 0.0001f)
            {
                Vector2 displacement = contact.mDirecciónContacto * contact.mMagnitudContacto;

                // **Dividir el desplazamiento de manera justa**
                colA.transform.position -= (Vector3)(displacement * 0.5f);
                colB.transform.position += (Vector3)(displacement * 0.5f);
            }
        }

        return collisionDetected;
    }



}

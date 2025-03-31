using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance;
    private List<CustomCollider> colliders = new List<CustomCollider>();
    public bool applyResolution = true;  
        
    private Dictionary<CustomCollider, SpriteRenderer> spriteRenderers = new Dictionary<CustomCollider, SpriteRenderer>();

    public void RegisterCollider(CustomCollider collider)
    {
        colliders.Add(collider);
        spriteRenderers[collider] = collider.GetComponent<SpriteRenderer>(); 
    }
    void Update()
    {
        HashSet<CustomCollider> collidingObjects = new HashSet<CustomCollider>();

        // recorrido de los colisionados en escena
        for (int i = 0; i < colliders.Count; i++)
        {
            var colA = colliders[i];
            for (int j = i + 1; j < colliders.Count; j++)
            {
                var colB = colliders[j];

                // comprobacion de colision
                if (ResolveCollision(colA, colB, applyResolution))
                {
                    // Colores para los objetos que colisionan
                    spriteRenderers[colA].color = Color.red;
                    spriteRenderers[colB].color = Color.green;

                    collidingObjects.Add(colA);
                    collidingObjects.Add(colB);
                }
            }
        }
        // Restablecer los colores de los colisionadores que no están colisionando
        foreach (var col in colliders)
        {
            if (!collidingObjects.Contains(col))
            {
                spriteRenderers[col].color = Color.white;
            }
        }
    }
    private bool ResolveCollision(CustomCollider colA, CustomCollider colB, bool applyResolution)
    {
        Contacto contact = new Contacto(Vector2.zero, Vector2.zero, 0);
        bool collisionDetected = false;

        // Resolución de colisiones, condicionada por el booleano activable
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

            contact.mDirecciónContacto = -contact.mDirecciónContacto;
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
        else if (colA.colliderType == CustomCollider.ColliderType.AABB && colB.colliderType == CustomCollider.ColliderType.OBB)
        {
            collisionDetected = CollisionFunctions.OBBToAABBResolution(
                colB.transform, colA.transform.position, colA.transform.localScale, out contact);
        }
        else if (colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.AABB)
        {
            collisionDetected = CollisionFunctions.OBBToAABBResolution(
                colA.transform, colB.transform.position, colB.transform.localScale, out contact);

            contact.mDirecciónContacto = -contact.mDirecciónContacto;
        }
        else if (colA.colliderType == CustomCollider.ColliderType.OBB && colB.colliderType == CustomCollider.ColliderType.OBB)
        {
            collisionDetected = CollisionFunctions.OBBToOBBResolution(colA.transform, colB.transform, out contact);
        }
        // Aplicar resolución si se detecta colisión
        if (collisionDetected && applyResolution)
        {
            if (contact.mMagnitudContacto > 0.0001f)
            {
                Vector2 displacement = contact.mDirecciónContacto * contact.mMagnitudContacto;

                colA.transform.position -= (Vector3)(displacement * 0.5f);
                colB.transform.position += (Vector3)(displacement * 0.5f);
            }
        }
        return collisionDetected;
    }
}

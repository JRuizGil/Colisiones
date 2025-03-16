using UnityEngine;
//clase que tienen los objetos para detectar las colisiones con el click y poder rotarlos
public class CustomCollider : MonoBehaviour
{
    public enum ColliderType { AABB, Circle, OBB }
    public ColliderType colliderType;
    private bool isDragging = false;
    private Vector2 offset;

    void Start()
    {
        CollisionManager.Instance.RegisterCollider(this);
    }
    void Update()
    {        
        HandleClick();        
    }
    //función para manejar la colision del click con los objetos de la escena.
    void HandleClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //el punto es el raton.
        if (Input.GetMouseButtonDown(0))
        {
            if (colliderType == ColliderType.AABB)
            {
                Vector2 min = (Vector2)transform.position - (Vector2)transform.localScale / 2;
                Vector2 max = (Vector2)transform.position + (Vector2)transform.localScale / 2;
                if (CollisionFunctions.PointToAABB(mousePos, min, max))
                {
                    isDragging = true;
                    offset = (Vector2)transform.position - mousePos;
                }
            }
            else if (colliderType == ColliderType.Circle)
            {
                float radius = transform.localScale.x / 2;
                if (CollisionFunctions.PointToCircle(mousePos, transform.position, radius))
                {
                    isDragging = true;
                    offset = (Vector2)transform.position - mousePos;
                }
            }
            else if (colliderType == ColliderType.OBB)
            {
                if (CollisionFunctions.PointToOBB(mousePos, transform))
                {
                    isDragging = true;
                    offset = (Vector2)transform.position - mousePos;
                }
            }
        }
        //evita el centrado automatico cuando se clicka en el objeto.
        if (isDragging)
        {
            transform.position = mousePos + offset;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        //rotacion de objetos mientas se mantiene el click
        if (isDragging && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)))
        {
            float rotationSpeed = 100f;
            float rotation = Input.GetKey(KeyCode.Q) ? rotationSpeed : -rotationSpeed;
            transform.Rotate(Vector3.forward, rotation * Time.deltaTime);
            if (colliderType == ColliderType.AABB)
            {
                colliderType = ColliderType.OBB;
            }
        }
    }
}
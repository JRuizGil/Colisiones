using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    public enum ColliderType { AABB, Circle, OBB }
    public ColliderType colliderType;

    private Vector2 size;
    private float radius;
    private float angle = 0f;
    private bool isDragging = false;
    private SpriteRenderer spriteRenderer;

    public Vector2 Size => size;
    public float Radius => radius;
    public bool IsDragging => isDragging;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (colliderType == ColliderType.Circle)
            radius = transform.localScale.x / 2f;
        else
            size = transform.localScale;

        // Registrar en el CollisionManager al iniciar
        CollisionManager.Instance?.Register(this);
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && IsMouseOver(mousePos))
        {
            isDragging = true;
            SetColor(Color.yellow);
        }

        if (isDragging)
        {
            transform.position = mousePos;

            if (Input.GetKey(KeyCode.Q)) angle += 1f;
            if (Input.GetKey(KeyCode.E)) angle -= 1f;
            transform.rotation = Quaternion.Euler(0, 0, angle);            
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SetColor(Color.white);
        }
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    private bool IsMouseOver(Vector2 mousePos)
    {
        Vector2 position = transform.position;
        switch (colliderType)
        {
            case ColliderType.AABB:
                return CollisionFunctions.PointToAABB(mousePos, position, size);
            case ColliderType.Circle:
                return CollisionFunctions.PointToCircle(mousePos, position, radius);
            case ColliderType.OBB:
                return CollisionFunctions.PointToOBB(mousePos, position, size, angle);
            default:
                return false;
        }
    }
}
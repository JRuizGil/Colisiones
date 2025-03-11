using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public GameObject squarePrefab;
    public GameObject circlePrefab;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.B)) Instantiate(squarePrefab, mousePos, Quaternion.identity);
        if (Input.GetKeyDown(KeyCode.C)) Instantiate(circlePrefab, mousePos, Quaternion.identity);
    }
}

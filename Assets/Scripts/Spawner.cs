using UnityEngine;
//clase para generar objetos en escena desde prefabs.
public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public GameObject squarePrefab;
    public GameObject circlePrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(squarePrefab, mousePos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Instantiate(circlePrefab, mousePos, Quaternion.identity);
        }
    }
}


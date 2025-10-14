using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject carPrefab;
    public int carCount = 10;
    public float moveSpeed = 1.5f;

    [Header("Spawn Area")]
    public float spawnRangeX = 8f;
    public float spawnRangeY = 5f;

    private GameObject[] cars;
    private Vector2[] directions;

    void Start()
    {
        if (carPrefab == null)
        {
            Debug.LogError("AgentSpawner: carPrefab not assigned!");
            return;
        }

        cars = new GameObject[carCount];
        directions = new Vector2[carCount];

        for (int i = 0; i < carCount; i++)
        {
            // Random position and direction
            Vector2 pos = new Vector2(
                Random.Range(-spawnRangeX, spawnRangeX),
                Random.Range(-spawnRangeY, spawnRangeY)
            );
            Vector2 randomDir = Random.insideUnitCircle.normalized;

            // Spawn car
            GameObject car = Instantiate(carPrefab, pos, Quaternion.identity, transform);

            car.transform.up = randomDir;

            cars[i] = car;
            directions[i] = randomDir;
        }
    }

    void Update()
    {
        if (cars == null) return;

        for (int i = 0; i < cars.Length; i++)
        {
            GameObject c = cars[i];
            if (c == null) continue;

            // Move car in its current direction
            c.transform.position += (Vector3)directions[i] * moveSpeed * Time.deltaTime;

            // Occasionally turn slightly
            if (Random.value < 0.01f)
            {
                float angle = Random.Range(-90f, 90f);
                directions[i] = Quaternion.Euler(0, 0, angle) * directions[i];
                c.transform.up = directions[i];
            }
        }
    }
}

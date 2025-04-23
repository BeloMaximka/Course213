using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Entity prefabs to choose from")]
    public GameObject[] entityPrefabs;

    [Tooltip("Center of the spawn circle")]
    public Transform spawnAnchor;

    [Tooltip("Seconds between each spawn attempt")]
    public float spawnInterval = 5f;

    [Tooltip("Maximum number of entities alive at once")]
    public int maxEntities = 10;

    [Tooltip("Radius of the spawn circles")]
    public float spawnRadius = 20f;

    [Tooltip("Minimum distance from center of the spawn circle")]
    public float minRadius = 5f;

    [Header("Ground Sampling")]
    [Tooltip("How high above the player to start the ground‐raycast")]
    public float raycastStartHeight = 100f;

    [Tooltip("Which layers count as ‘ground’ (terrain, meshes, etc.)")]
    public LayerMask groundLayerMask;

    [Tooltip("Enable debug info")]
    public bool isDebug;

    // Internal list to track spawned enemies
    private readonly List<GameObject> spawnedEntities = new List<GameObject>();
    private Terrain terrain;

    void Awake()
    {
        // Cache active terrain
        terrain = Terrain.activeTerrain;
        if (terrain == null)
            Debug.LogWarning("No active Terrain found. Spawns will default to y=0.");
    }

    void Start()
    {
        // Start periodic spawning
        InvokeRepeating(nameof(SpawnAttempt), 0f, spawnInterval);
    }

    void SpawnAttempt()
    {
        // Clean up destroyed enemies
        spawnedEntities.RemoveAll(e => e == null);

        // Don't spawn if at cap
        if (spawnedEntities.Count >= maxEntities)
            return;

        // Choose a random prefab
        var prefab = entityPrefabs[Random.Range(0, entityPrefabs.Length)];
        if (prefab == null) return;

        // Compute spawn position
        Vector3 spawnPos = GetRandomPointOnGround(prefab);

        // Instantiate and track
        var enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedEntities.Add(enemy);
    }

    Vector2 GetRandomPointInCircle()
    {
        Vector2 insideUnitCircle = Random.insideUnitCircle;
        return Random.insideUnitCircle * (spawnRadius - minRadius) + insideUnitCircle * minRadius;
    }

    Vector3 GetRandomPointOnGround(GameObject enemy)
    {
        Vector2 rnd = GetRandomPointInCircle();
        Vector3 candidate = spawnAnchor.position + new Vector3(rnd.x, raycastStartHeight, rnd.y);


        Renderer col = enemy.GetComponentInChildren<Renderer>();
        float height = (col != null ? col.bounds.size.y : 100f) + 1f;
        if (isDebug)
        {
            Debug.Log($"Entity height: {height}");
            Debug.DrawLine(candidate, candidate + (2f * raycastStartHeight * Vector3.down), Color.blue, 1000);
        }

        if (Physics.Raycast(
                candidate,
                Vector3.down,
                out RaycastHit hit,
                raycastStartHeight * 2f,
                groundLayerMask
            ))
        {
            if (isDebug)
            {
                Debug.Log("Entity spawn position at terrain raycast y-level");
            }
            return hit.point + Vector3.up * height;
        }

        if (isDebug)
        {
            Debug.Log("Entity spawn position at player y-level");
        }
        return spawnAnchor.position + Vector3.up * height;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnAnchor != null && isDebug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnAnchor.position, spawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnAnchor.position, minRadius);
        }
    }
}

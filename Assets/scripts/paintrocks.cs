using UnityEngine;

public class RocketScatter : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] rocketPrefabs;   // your rocks/rockets

    [Header("Terrain")]
    public Terrain terrain;
    public int count = 500;

    [Header("Placement rules")]
    [Tooltip("Skip spots steeper than this angle (degrees). 90 = ignore slope.")]
    [Range(0f, 90f)] public float maxSlopeAngle = 35f;
    [Tooltip("Random uniform scale range applied to each spawned rock.")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.3f);
    [Tooltip("How many times to retry a point if it fails the slope check.")]
    public int maxRetriesPerRock = 5;

    [Header("Organization")]
    [Tooltip("Parent all spawned rocks under a child object so they're easy to find/clear.")]
    public bool groupUnderParent = true;

    void Start()
    {
        Scatter();
    }

    [ContextMenu("Scatter Rocks")]
    public void Scatter()
    {
        if (terrain == null || rocketPrefabs == null || rocketPrefabs.Length == 0)
        {
            Debug.LogWarning("RocketScatter: Terrain or prefab list not assigned.", this);
            return;
        }

        // Clear previously spawned rocks if re-running in editor
        Transform parent = transform;
        if (groupUnderParent)
        {
            Transform existing = transform.Find("SpawnedRocks");
            if (existing != null) DestroyImmediateOrRuntime(existing.gameObject);

            GameObject container = new GameObject("SpawnedRocks");
            container.transform.SetParent(transform, false);
            parent = container.transform;
        }

        TerrainData td = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = td.size;

        int spawned = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3? point = FindValidPoint(td, terrainPos, terrainSize);
            if (point == null) continue; // gave up after retries, skip this one

            Vector3 pos = point.Value;
            GameObject prefab = rocketPrefabs[Random.Range(0, rocketPrefabs.Length)];

            GameObject rock = Instantiate(
                prefab,
                pos,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f),
                parent
            );

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            rock.transform.localScale *= scale;

            spawned++;
        }

        Debug.Log($"RocketScatter: spawned {spawned}/{count} rocks on '{terrain.name}'.", this);
    }

    Vector3? FindValidPoint(TerrainData td, Vector3 terrainPos, Vector3 terrainSize)
    {
        for (int attempt = 0; attempt < maxRetriesPerRock; attempt++)
        {
            // Local offset within the terrain, 0..size
            float localX = Random.Range(0f, terrainSize.x);
            float localZ = Random.Range(0f, terrainSize.z);

            // World-space X/Z (this was the bug: must include terrain position before sampling)
            float worldX = localX + terrainPos.x;
            float worldZ = localZ + terrainPos.z;

            // SampleHeight wants a world-space position (Y is ignored on input)
            float worldY = terrain.SampleHeight(new Vector3(worldX, 0f, worldZ)) + terrainPos.y;

            // Normalized coords for slope/steepness lookup (0..1 across the terrain)
            float normX = localX / terrainSize.x;
            float normZ = localZ / terrainSize.z;
            float steepness = td.GetSteepness(normX, normZ); // degrees

            if (steepness <= maxSlopeAngle)
            {
                return new Vector3(worldX, worldY, worldZ);
            }
        }
        return null; // couldn't find a flat-enough spot in time
    }

    void DestroyImmediateOrRuntime(GameObject go)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            DestroyImmediate(go);
            return;
        }
#endif
        Destroy(go);
    }
}
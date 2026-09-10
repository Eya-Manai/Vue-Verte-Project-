using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GPUGrassSpawner : MonoBehaviour
{
    [System.Serializable]
    public class GrassType
    {
        [Tooltip("Drag the Mesh from your grass prefab's Mesh Filter here")]
        public Mesh mesh;
        [Tooltip("Drag the Material from your grass prefab's Mesh Renderer here (must have 'Enable GPU Instancing' checked)")]
        public Material material;
    }

    [Header("Grass Types (mesh + material extracted from your prefabs)")]
    public GrassType[] grassTypes;

    [Header("Ground")]
    public Transform groundPlane;

    [Header("Spawn Settings")]
    public int grassCount = 50000;
    public float grassHeightOffset = -20.67f;

    [Header("Base Scale (match your original prefab's Transform scale, e.g. 100)")]
    public float baseScale = 100f;

    [Header("Random Scale (multiplied on top of Base Scale)")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Rendering")]
    public bool castShadows = true;
    public bool receiveShadows = true;

    // Unity's hard limit per DrawMeshInstanced call
    private const int MAX_INSTANCES_PER_BATCH = 1023;

    private class InstanceBatch
    {
        public Mesh mesh;
        public Material material;
        public List<Matrix4x4[]> chunks = new List<Matrix4x4[]>();
    }

    private readonly List<InstanceBatch> instanceBatches = new List<InstanceBatch>();

    void Start()
    {
        GenerateGrass();
    }

    void GenerateGrass()
    {
        if (groundPlane == null)
        {
            Debug.LogError("Ground Plane is missing!");
            return;
        }

        if (grassTypes == null || grassTypes.Length == 0)
        {
            Debug.LogError("No grass types assigned!");
            return;
        }

        Renderer groundRenderer = groundPlane.GetComponent<Renderer>();
        if (groundRenderer == null)
        {
            Debug.LogError("Ground needs a Mesh Renderer!");
            return;
        }

        float width = groundRenderer.bounds.size.x;
        float length = groundRenderer.bounds.size.z;
        float groundY = groundRenderer.bounds.max.y;

        // Group generated matrices by grass type index
        var matricesByType = new List<Matrix4x4>[grassTypes.Length];
        for (int t = 0; t < grassTypes.Length; t++)
            matricesByType[t] = new List<Matrix4x4>();

        for (int i = 0; i < grassCount; i++)
        {
            float x = Random.Range(groundPlane.position.x - width / 2f, groundPlane.position.x + width / 2f);
            float z = Random.Range(groundPlane.position.z - length / 2f, groundPlane.position.z + length / 2f);

            Vector3 position = new Vector3(x, groundY + grassHeightOffset, z);
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            float scale = Random.Range(minScale, maxScale) * baseScale;

            int typeIndex = Random.Range(0, grassTypes.Length);
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one * scale);
            matricesByType[typeIndex].Add(matrix);
        }

        // Split each type's matrices into batches of <= 1023
        for (int t = 0; t < grassTypes.Length; t++)
        {
            if (grassTypes[t].mesh == null || grassTypes[t].material == null)
            {
                Debug.LogWarning($"Grass type {t} is missing a mesh or material, skipping.");
                continue;
            }

            var batch = new InstanceBatch
            {
                mesh = grassTypes[t].mesh,
                material = grassTypes[t].material
            };

            List<Matrix4x4> all = matricesByType[t];
            for (int i = 0; i < all.Count; i += MAX_INSTANCES_PER_BATCH)
            {
                int count = Mathf.Min(MAX_INSTANCES_PER_BATCH, all.Count - i);
                Matrix4x4[] chunk = all.GetRange(i, count).ToArray();
                batch.chunks.Add(chunk);
            }

            instanceBatches.Add(batch);
        }

        Debug.Log($"GPU grass generated: {grassCount} blades across {instanceBatches.Count} type(s).");
    }

    void Update()
    {
        ShadowCastingMode shadowMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;

        foreach (var batch in instanceBatches)
        {
            foreach (var matrixArray in batch.chunks)
            {
                Graphics.DrawMeshInstanced(
                    batch.mesh,
                    0,
                    batch.material,
                    matrixArray,
                    matrixArray.Length,
                    null,
                    shadowMode,
                    receiveShadows
                );
            }
        }
    }
}
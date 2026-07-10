using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    [Header("Grass Prefabs")]
    public GameObject[] grassPrefabs;

    [Header("Ground")]
    public Transform groundPlane;

    [Header("Spawn Settings")]
    public int grassCount = 1000;

    [Header("Grass Position Offset")]
    public float grassHeightOffset = -20.67f;

    [Header("Random Scale")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;


    void Start()
    {
        SpawnGrass();
    }


    void SpawnGrass()
    {
        if (groundPlane == null)
        {
            Debug.LogError("Ground Plane is missing!");
            return;
        }

        if (grassPrefabs.Length == 0)
        {
            Debug.LogError("No grass prefab assigned!");
            return;
        }


        Renderer groundRenderer = groundPlane.GetComponent<Renderer>();

        if (groundRenderer == null)
        {
            Debug.LogError("Ground needs a Mesh Renderer!");
            return;
        }


        // Get real plane dimensions
        float width = groundRenderer.bounds.size.x;
        float length = groundRenderer.bounds.size.z;

        // Get top surface of plane
        float groundY = groundRenderer.bounds.max.y;


        for (int i = 0; i < grassCount; i++)
        {
            // Random position on plane
            float x = Random.Range(
                groundPlane.position.x - width / 2,
                groundPlane.position.x + width / 2
            );

            float z = Random.Range(
                groundPlane.position.z - length / 2,
                groundPlane.position.z + length / 2
            );


            // Apply grass vertical correction
            Vector3 position = new Vector3(
                x,
                groundY + grassHeightOffset,
                z
            );


            // Choose random grass prefab
            GameObject prefab =
                grassPrefabs[Random.Range(0, grassPrefabs.Length)];


            GameObject grass =
                Instantiate(
                    prefab,
                    position,
                    Quaternion.identity
                );


            // Random rotation
            grass.transform.rotation =
                Quaternion.Euler(
                    0,
                    Random.Range(0,360),
                    0
                );


            // Random size variation
            float scale =
                Random.Range(minScale, maxScale);


            grass.transform.localScale =
                prefab.transform.localScale * scale;
        }
    }
}
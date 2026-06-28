using UnityEngine;

public class RocketScatter : MonoBehaviour
{
    public GameObject[] rocketPrefabs; // your two rockets
    public Terrain terrain;
    public int count = 50;

    void Start()
    {
        TerrainData td = terrain.terrainData;
        for (int i = 0; i < count; i++)
        {
            // Random position within terrain bounds
            float x = Random.Range(0f, td.size.x);
            float z = Random.Range(0f, td.size.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            Vector3 pos = new Vector3(x, y, z) + terrain.transform.position;
            GameObject prefab = rocketPrefabs[Random.Range(0, rocketPrefabs.Length)];
            Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
    }
}
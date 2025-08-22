using System.Collections.Generic;
using UnityEngine;

public class FishboneSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject fishbonePrefab;    // Prefab coin/fishbone
    public Transform player;             // Player
    public float spawnDistance = 40f;    // Spawn trước mặt player bao xa
    public float minSpawnInterval = 8f;  // Khoảng cách Z tối thiểu giữa các cụm
    public float maxSpawnInterval = 15f; // Khoảng cách Z tối đa giữa các cụm
    public int maxFishbones = 30;        // Giới hạn số lượng tồn tại
    public float destroyDistance = 50f;  // Nếu coin tụt sau player bao xa thì xóa

    [Header("Coin Line Settings")]
    public int lineLength = 5;           // Bao nhiêu coin trong một cụm
    public float lineSpacing = 1.5f;     // Khoảng cách giữa coin trong cụm
    public float coinHeight = 1.5f;      // Độ cao bay trên không

    private float nextSpawnZ = 0f;
    private float currentInterval;

    private List<GameObject> activeFishbones = new List<GameObject>();

    // Lane 3 hàng: trái, giữa, phải
    private float[] lanes = { -2f, 0f, 2f };

    void Start()
    {
        currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnZ = player.position.z + spawnDistance;
    }

    void Update()
    {
        // Spawn coin liên tục khi còn dưới giới hạn
        if (activeFishbones.Count < maxFishbones && player.position.z + spawnDistance >= nextSpawnZ)
        {
            SpawnLine();
            currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnZ += currentInterval;
        }

        // Cleanup: coin nào quá xa phía sau player thì xóa
        for (int i = activeFishbones.Count - 1; i >= 0; i--)
        {
            if (player.position.z - activeFishbones[i].transform.position.z > destroyDistance)
            {
                Destroy(activeFishbones[i]);
                activeFishbones.RemoveAt(i);
            }
        }
    }

    void SpawnLine()
    {
        float xPos = lanes[Random.Range(0, lanes.Length)]; // random lane
        float yPos = coinHeight;                          // bay cố định trên không

        for (int i = 0; i < lineLength; i++)
        {
            Vector3 pos = new Vector3(xPos, yPos, nextSpawnZ + i * lineSpacing);
            GameObject fb = Instantiate(fishbonePrefab, pos, Quaternion.identity);
            activeFishbones.Add(fb);
        }
    }
}

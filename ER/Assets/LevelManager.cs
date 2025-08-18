using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] mapPrefabs;
    public Transform player;
    public int mapsAhead = 5;

    private List<GameObject> spawnedMaps = new List<GameObject>();
    private Transform lastExit;

    void Start()
    {
        for (int i = 0; i < mapsAhead; i++)
        {
            SpawnMap();
        }
    }

    void Update()
    {
        // Spawn thêm nếu player gần điểm exit cuối cùng
        if (lastExit != null && player.position.z + 30f > lastExit.position.z)
        {
            SpawnMap();
            RemoveOldMap();
        }
    }

    void SpawnMap()
    {
        GameObject prefab = mapPrefabs[Random.Range(0, mapPrefabs.Length)];
        GameObject newMap = Instantiate(prefab);

        Transform entry = FindDeepChild(newMap.transform, "Entry");
        Transform exit = FindDeepChild(newMap.transform, "Exit");

        if (entry == null || exit == null)
        {
            Debug.LogError($"Prefab {prefab.name} thiếu Entry hoặc Exit");
            Destroy(newMap); // Xoá nếu không hợp lệ
            return;
        }

        if (lastExit == null)
        {
            // Map đầu tiên, đặt tại vị trí gốc
            newMap.transform.position = Vector3.zero;
        }
        else
        {
            // Dịch chuyển sao cho Entry trùng với vị trí lastExit
            Vector3 offset = lastExit.position - entry.position;
            newMap.transform.position += offset;
        }

        lastExit = FindDeepChild(newMap.transform, "Exit");
        spawnedMaps.Add(newMap);
    }

    void RemoveOldMap()
    {
        if (spawnedMaps.Count > mapsAhead)
        {
            Destroy(spawnedMaps[0]);
            spawnedMaps.RemoveAt(0);
        }
    }

    // Hàm tìm sâu
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    // Vẽ line debug trong Scene View
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (GameObject map in spawnedMaps)
        {
            Transform entry = FindDeepChild(map.transform, "Entry");
            Transform exit = FindDeepChild(map.transform, "Exit");

            if (entry != null && exit != null)
            {
                Gizmos.DrawLine(entry.position, exit.position);
            }
        }
    }
}
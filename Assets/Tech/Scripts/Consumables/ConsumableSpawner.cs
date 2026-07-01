using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableSpawner : MonoBehaviour
{
    [SerializeField] private List<ConsumableData> consumables;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject meshParent;
    [SerializeField] private Collider roomArea;
    [SerializeField] private float overlapRadius = 0.4f;

    [Range(0f, 1f)]
    [SerializeField] private float chanceToSpawnAny = 0.6f;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float overlapCheckHeight = 0.5f;

    private System.Random _rnd;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(spawnDelay);

        _rnd = new System.Random(RunManager.Seed ^ gameObject.GetInstanceID());

        TrySpawnConsumable();
    }

    private void TrySpawnConsumable()
    {
        if (consumables == null || consumables.Count == 0) return;
        if (_rnd.NextDouble() > chanceToSpawnAny) return;

        List<ConsumableData> pool = BuildPool();
        if (pool.Count == 0) return;

        ConsumableData chosen = pool[_rnd.Next(pool.Count)];

        List<Transform> available = GetAvailableSpawnPoints();
        if (available.Count == 0) return;

        Transform point = available[_rnd.Next(available.Count)];
        Spawn(chosen, point.position);
    }

    private List<ConsumableData> BuildPool()
    {
        List<ConsumableData> pool = new();
        foreach (ConsumableData data in consumables)
        {
            if (data.Prefab == null) continue;
            if (_rnd.NextDouble() <= data.chanceToSpawn)
                pool.Add(data);
        }
        return pool;
    }

    private List<Transform> GetAvailableSpawnPoints()
    {
        List<Transform> available = new();
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            Vector3 testPos = point.position + Vector3.up * overlapCheckHeight;
            Collider[] hits = Physics.OverlapSphere(testPos, overlapRadius);

            bool blocked = false;
            foreach (Collider hit in hits)
            {
                if (hit == roomArea) continue; 
                blocked = true;
                break;
            }

            if (!blocked) available.Add(point);
        }
        return available;
    }

    private void Spawn(ConsumableData data, Vector3 position)
    {
        Transform parent = meshParent != null ? meshParent.transform : transform;
        Instantiate(data.Prefab, position, Quaternion.identity, parent);

        if (meshParent != null)
            meshParent.SetActive(true);
    }

}

[System.Serializable]
public class ConsumableData
{
    public GameObject Prefab;

    [Range(0f, 1f)]
    public float chanceToSpawn = 0.5f;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableSpawner : MonoBehaviour
{
    [SerializeField] private List<ConsumableData> consumables;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject meshParent;
    [SerializeField] private float overlapRadius = 0.4f;
    [SerializeField] private LayerMask blockingLayers;

    [Range(0f, 1f)]
    [SerializeField] private float chanceToSpawnAny = 0.6f;
    [SerializeField] private float spawnDelay = 0.7f;

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
            if (Physics.OverlapSphere(point.position, overlapRadius, blockingLayers).Length == 0)
                available.Add(point);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(point.position, overlapRadius);
        }
    }
#endif
}

[System.Serializable]
public class ConsumableData
{
    public GameObject Prefab;

    [Range(0f, 1f)]
    public float chanceToSpawn = 0.5f;
}
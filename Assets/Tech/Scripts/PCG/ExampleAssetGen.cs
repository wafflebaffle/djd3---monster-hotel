using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExampleAssetGen : MonoBehaviour
{
    [SerializeField] private float stepSize = 1.0f;
    [SerializeField] private float minObjectDistance = 1.0f;
    [SerializeField] private float minEdgeDistance = 1.0f;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask unwalkableMask;
    [Header("Spawn Rules")]
    public List<SpawnRule> spawnRules;
    
    private List<Vector3> walkablePositions;
    private List<Vector3> wallAdjacentPositions;
    private List<Vector3> cornerPositions;
    private List<Bounds> gapBounds;
    private Bounds roomBounds;
    private System.Random rnd;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"No Collider found on {gameObject.name}. Please add a Collider to define room bounds.");
            return;
        }
        roomBounds = col.bounds;
        
        gapBounds = FindGapBounds();
        
        if (RunManager.Seed != 0)
            rnd = new System.Random(RunManager.Seed);
        else
            rnd = new System.Random();

        // Sample all walkable positions once
        walkablePositions = GetValidFloorPositions(roomBounds);
        wallAdjacentPositions = GetWallAdjacentPositions();
        cornerPositions = GetCornerPositions();
        
        if (walkablePositions.Count == 0)
        {
            Debug.LogError($"No walkable positions found in {gameObject.name}! Check floorLayer mask.");
            return;
        }

        GenerateAll();
    }

    private List<Vector3> GetValidFloorPositions(Bounds roomBounds)
    {
        List<Vector3> validSpots = new List<Vector3>();

        for (float x = roomBounds.min.x; x <= roomBounds.max.x; x += stepSize)
        {
            for (float z = roomBounds.min.z; z <= roomBounds.max.z; z += stepSize)
            {
                Vector3 origin = new Vector3(x, roomBounds.max.y + 1f, z);
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
                {
                    // Check if this position is not inside a gap
                    if (!IsInsideGap(hit.point))
                        validSpots.Add(hit.point);
                }
            }
        }
        
        Debug.Log($"Found {validSpots.Count} walkable positions");
        return validSpots;
    }

    private bool IsInsideGap(Vector3 point)
    {
        return Physics.CheckSphere(point, 0.5f, unwalkableMask);
    }

    private List<Vector3> GetWallAdjacentPositions()
    {
        List<Vector3> wallSpots = new List<Vector3>();
        float wallDistance = 0.5f;
        
        foreach (Vector3 pos in walkablePositions)
        {
            float distToBoundsX = Mathf.Min(
                Mathf.Abs(pos.x - roomBounds.min.x),
                Mathf.Abs(pos.x - roomBounds.max.x)
            );
            float distToBoundsZ = Mathf.Min(
                Mathf.Abs(pos.z - roomBounds.min.z),
                Mathf.Abs(pos.z - roomBounds.max.z)
            );
            
            if (distToBoundsX < wallDistance || distToBoundsZ < wallDistance)
                wallSpots.Add(pos);
        }
        
        return wallSpots;
    }

    private List<Vector3> GetCornerPositions()
    {
        List<Vector3> cornerSpots = new List<Vector3>();
        float cornerDistance = 0.8f;
        
        foreach (Vector3 pos in walkablePositions)
        {
            float distToCorner = Mathf.Min(
                Vector3.Distance(pos, new Vector3(roomBounds.min.x, 0, roomBounds.min.z)),
                Vector3.Distance(pos, new Vector3(roomBounds.min.x, 0, roomBounds.max.z)),
                Vector3.Distance(pos, new Vector3(roomBounds.max.x, 0, roomBounds.min.z)),
                Vector3.Distance(pos, new Vector3(roomBounds.max.x, 0, roomBounds.max.z))
            );
            
            if (distToCorner < cornerDistance)
                cornerSpots.Add(pos);
        }
        
        return cornerSpots;
    }

    void GenerateAll()
    {
        List<Vector3> allSpawns = new List<Vector3>();

        foreach (var rule in spawnRules)
        {
            if (rule == null || rule.Prefab == null)
            {
                Debug.LogWarning($"Invalid spawn rule in {gameObject.name}");
                continue;
            }
            
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = rule.MaxCount * 20;
            
            List<Vector3> ruleSpawns = new List<Vector3>();

            while (spawned < rule.MaxCount && attempts < maxAttempts && walkablePositions.Count > 0)
            {
                Vector3 candidate = GetPositionByStrategy(rule);

                if (IsValidForRule(candidate, rule, allSpawns, ruleSpawns))
                {
                    Quaternion rotation = GetRotationForPlacement(candidate, rule);
                    Instantiate(rule.Prefab, candidate, rotation, transform);
                    allSpawns.Add(candidate);
                    ruleSpawns.Add(candidate);
                    spawned++;
                }
                attempts++;
            }
            
            if (spawned < rule.MaxCount)
                Debug.LogWarning($"Only spawned {spawned}/{rule.MaxCount} of {rule.Category} in {gameObject.name}");
        }
    }

    private Vector3 GetPositionByStrategy(SpawnRule rule)
    {
        switch (rule.PlacementStrategy)
        {
            case PlacementStrategy.AgainstWall:
                if (wallAdjacentPositions.Count > 0)
                    return wallAdjacentPositions[rnd.Next(wallAdjacentPositions.Count)];
                break;
                
            case PlacementStrategy.InCorner:
                if (cornerPositions.Count > 0)
                    return cornerPositions[rnd.Next(cornerPositions.Count)];
                break;
                
            case PlacementStrategy.Center:
                return GetCenterPosition();
                
            case PlacementStrategy.Scattered:
            default:
                return GetRandomWalkablePosition();
        }
        
        return GetRandomWalkablePosition();
    }

    private Vector3 GetCenterPosition()
    {
        Vector3 center = roomBounds.center;
        Vector3 closest = walkablePositions.OrderBy(p => Vector3.Distance(p, center)).FirstOrDefault();
        return closest;
    }

    private Quaternion GetRotationForPlacement(Vector3 position, SpawnRule rule)
    {
        if (!rule.RotateToFaceWall)
            return Quaternion.identity;
        
        float distToLeft = Mathf.Abs(position.x - roomBounds.min.x);
        float distToRight = Mathf.Abs(position.x - roomBounds.max.x);
        float distToFront = Mathf.Abs(position.z - roomBounds.min.z);
        float distToBack = Mathf.Abs(position.z - roomBounds.max.z);
        
        float minDist = Mathf.Min(distToLeft, distToRight, distToFront, distToBack);
        
        if (minDist == distToLeft)
            return Quaternion.LookRotation(Vector3.right);
        if (minDist == distToRight)
            return Quaternion.LookRotation(Vector3.left);
        if (minDist == distToFront)
            return Quaternion.LookRotation(Vector3.forward);
        if (minDist == distToBack)
            return Quaternion.LookRotation(Vector3.back);
        
        return Quaternion.identity;
    }

    bool IsValidForRule(Vector3 pos, SpawnRule rule, List<Vector3> allSpawns, List<Vector3> ruleSpawns)
    {
        // Check distance to all spawned objects
        foreach (var existing in allSpawns)
            if (Vector3.Distance(pos, existing) < rule.MinDistanceFromOthers)
                return false;
        
        // Check distance to same category objects
        foreach (var existing in ruleSpawns)
            if (Vector3.Distance(pos, existing) < rule.MinDistanceFromOthers * 0.8f)
                return false;
        
        // Check distance to gaps
        if (DistanceToNearestGap(pos, gapBounds) < rule.MinDistanceFromGapEdge)
            return false;
        
        // Not inside a gap
        if (IsInsideGap(pos))
            return false;
        
        // Wall check for against-wall items
        if (rule.PlacementStrategy == PlacementStrategy.AgainstWall)
        {
            float distToWall = Mathf.Min(
                Mathf.Abs(pos.x - roomBounds.min.x),
                Mathf.Abs(pos.x - roomBounds.max.x),
                Mathf.Abs(pos.z - roomBounds.min.z),
                Mathf.Abs(pos.z - roomBounds.max.z)
            );
            if (distToWall > 0.7f) return false;
        }
        
        return true;
    }

    Vector3 GetRandomWalkablePosition()
    {
        if (walkablePositions.Count == 0)
            return Vector3.zero;
        return walkablePositions[rnd.Next(0, walkablePositions.Count)];
    }

    private List<Bounds> FindGapBounds()
    {
        List<Bounds> gaps = new List<Bounds>();
        
        Collider[] triggers = GetComponentsInChildren<Collider>();
        int gapLayer = LayerMask.NameToLayer("Gap");
        
        if (gapLayer == -1)
        {
            Debug.LogWarning("No 'Gap' layer found. Please create one in Tags & Layers.");
            return gaps;
        }
        
        foreach (Collider col in triggers)
        {
            if (col.isTrigger && col.gameObject.layer == gapLayer)
            {
                gaps.Add(col.bounds);
            }
        }
        
        Debug.Log($"Found {gaps.Count} gap bounds");
        return gaps;
    }

    private float DistanceToNearestGap(Vector3 position, List<Bounds> gapBounds)
    {
        if (gapBounds.Count == 0)
            return 9999f;
            
        float minDistance = float.MaxValue;
        
        foreach (Bounds gap in gapBounds)
        {
            Vector3 closestPoint = gap.ClosestPoint(position);
            float distance = Vector3.Distance(position, closestPoint);
            
            if (distance < minDistance)
                minDistance = distance;
        }
        
        return minDistance;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (walkablePositions != null)
        {
            Gizmos.color = Color.green;
            foreach (var pos in walkablePositions)
            {
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }
        
        if (gapBounds != null)
        {
            Gizmos.color = Color.red;
            foreach (var gap in gapBounds)
            {
                Gizmos.DrawWireCube(gap.center, gap.size);
            }
        }
    }
}

[System.Serializable]
public class SpawnRule
{
    public SpawnCategory Category;
    public GameObject Prefab;
    public int MaxCount = 1;
    public float MinDistanceFromOthers = 1.5f;
    public float MinDistanceFromGapEdge = 0.5f;
    
    // New fields for better placement
    public PlacementStrategy PlacementStrategy = PlacementStrategy.Scattered;
    public bool RotateToFaceWall = false;
}

public enum SpawnCategory { Asset, Enemy, Consumable }
public enum PlacementStrategy
{
    Scattered,
    AgainstWall,
    InCorner,
    Center
}
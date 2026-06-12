using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine;

public class AssetsGenerator : MonoBehaviour 
{
    // List of all assets, adjustable in editor;
    [SerializeField] private List<Assets> assets;
    // Area of the room;
    [SerializeField] private Collider roomArea;
    // Every mesh in the gameobject;
    [SerializeField] private GameObject allMeshes;
    // Size between in point;
    [SerializeField] private float stepSize;
    // Floor layer for raycast;
    [SerializeField] private LayerMask floorLayer;
    // The chance of spawning an asset during the loop;
    [SerializeField] private float chanceToSpawnAsset;
    /// Max interarions to spawn assets to force the loop to stop;
    [SerializeField] private int maxIterations;
    // Random from System;
    private System.Random rnd;
    // Boundaries of the room;
    private Bounds _roomsBounds;
    // A list of all points;
    private HashSet<Vector3> _points = new();
    // A list of all unavailable points;
    private HashSet<Vector3> _unavailablePoints = new();
    // Dictionary for types of positions for each side of the room, storing their position as keys and direction as values;
    private Dictionary<Vector3, Vector3> _corners = new (), _walls = new (), _center = new ();
    // Dictionary for all position of the room, storing position as keys and if occupied as value;
    private Dictionary<Vector3, bool> _positions = new();
    // Cache storage for the quantity of assets that remais to dispose;
    private int _remainingAssets;
    // Reference for the NavMeshSurface, to bake it and got points from it;
    private NavMeshSurface _surfaceAI;

    /// <summary>
    /// It set ups the NavMeshSurface and bake it, instance the random using our universal seed, get the points on the room
    /// and dispose all assets.
    /// </summary>
    /// <returns> Returns a WaitForSeconds of 1 second, since the room doesn't start directly on the right position. </returns>
    private IEnumerator Start()
    {
        yield return null;

        Debug.Log("Loading");

        _surfaceAI = GetComponent<NavMeshSurface>();
        _surfaceAI.BuildNavMesh();

        rnd = new System.Random(RunManager.Seed);
        _roomsBounds = roomArea.bounds;

        foreach(Assets asset in assets)
            asset.RemainingCount = asset.MaxCount;

        GetPositions();
        Debug.Log("How many points: " + _points.Count);

        foreach (Vector3 point in _points)
        {
            _positions.Add(point, false);
        }

        OrganizePositions();
        DisposeAssets();

        allMeshes.SetActive(false);
    }

    /// <summary>
    /// Save all points inside the boundaries of the room on the _points list. It uses the NavMeshSurface to check if that position exists.
    /// </summary>
    private void GetPositions()
    {
        for (float x = _roomsBounds.min.x; x <= _roomsBounds.max.x; x += stepSize)
        {
            for (float z = _roomsBounds.min.z; z <= _roomsBounds.max.z; z += stepSize)
            {
                Vector3 samplePoint = new Vector3(x, _roomsBounds.center.y, z);
                
                float sampleRadius = stepSize * 2f;
                if (NavMesh.SamplePosition(samplePoint, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
                {
                    float roundedX = Mathf.Round(hit.position.x / stepSize) * stepSize;
                    float roundedZ = Mathf.Round(hit.position.z / stepSize) * stepSize;
                    float roundedY = Mathf.Round(hit.position.y / stepSize) * stepSize;
                    Vector3 roundedPoint = new Vector3(roundedX, roundedY, roundedZ);

                    if (Physics.Raycast(roundedPoint + Vector3.up, Vector3.down, out RaycastHit rayHit, 2f, floorLayer))
                        if (rayHit.transform.TryGetComponent(out Temp _))
                            _unavailablePoints.Add(roundedPoint);
    
                    _points.Add(roundedPoint);
                }
            }
        }
    }

    /// <summary>
    /// A boolean check if a position is available.
    /// </summary>
    /// <param name="point"></param>
    /// <returns> return true if a point is occupied </returns>
    private bool IsUnavailable(Vector3 point)
    {
        foreach (Vector3 p in _unavailablePoints)
        {
            if (Mathf.Approximately(p.x, point.x) && 
                Mathf.Approximately(p.z, point.z))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Organize every point for diferent sides of the room, for the dictionaries from above: _center, _wall and _corner;
    /// Center points do not have directions.
    /// Wall points store as values the direction they are facing.
    /// Corner points store as values the direction they are facing.
    /// </summary>
    private void OrganizePositions()
    {
        foreach (Vector3 point in _points)
        {
            bool hasXPlus = _points.Contains(point + new Vector3(stepSize, 0, 0));
            bool hasXMinus = _points.Contains(point + new Vector3(-stepSize, 0, 0));
            bool hasZPlus = _points.Contains(point + new Vector3(0, 0, stepSize));
            bool hasZMinus = _points.Contains(point + new Vector3(0, 0, -stepSize));
        
            int neighborCount = (hasXPlus ? 1 : 0) + (hasXMinus ? 1 : 0) + 
                                (hasZPlus ? 1 : 0) + (hasZMinus ? 1 : 0);
        
            if (neighborCount <= 1)
            {
                if(hasXPlus) _corners.Add(point, Vector3.right);
                else if(hasXMinus) _corners.Add(point, Vector3.left);
                else if(hasZPlus) _corners.Add(point, Vector3.forward);
                else if(hasZMinus) _corners.Add(point, Vector3.back);
            }
            else if (neighborCount == 2)
            {
                if ((hasXPlus || hasXMinus) && (hasZPlus || hasZMinus))
                {
                    if (hasXPlus && hasZPlus) _corners.Add(point, new Vector3(1, 0, 1));
                    else if (hasXPlus && hasZMinus) _corners.Add(point, new Vector3(1, 0, -1));
                    else if (hasXMinus && hasZPlus) _corners.Add(point, new Vector3(-1, 0, 1));
                    else if (hasXMinus && hasZMinus) _corners.Add(point, new Vector3(-1, 0, -1));
                }
                else
                {
                    if (hasXPlus) _walls.Add(point, Vector3.back);
                    else if (hasXMinus) _walls.Add(point, Vector3.forward);
                    else if (hasZPlus) _walls.Add(point, Vector3.right);
                    else if (hasZMinus) _walls.Add(point, Vector3.left);
                }
            }
            else if (neighborCount == 3)
            {
                if (!hasXPlus && hasXMinus && hasZPlus && hasZMinus) _walls.Add(point, Vector3.back);
                else if (!hasXMinus && hasXPlus && hasZPlus && hasZMinus) _walls.Add(point, Vector3.forward);
                else if (!hasZPlus && hasXPlus && hasXMinus && hasZMinus) _walls.Add(point, Vector3.right);
                else if (!hasZMinus && hasXPlus && hasXMinus && hasZPlus) _walls.Add(point, Vector3.left);
            }
            else if (neighborCount == 4)
            {
                _center.Add(point, Vector3.zero);
            }
        }
    }
  
    /// <summary>
    /// Dispose assets on the room, storing them separatly by their preference placement, and then doing a loop
    /// until reach the max interation.
    /// </summary>
    private void DisposeAssets()
    {
        List<Assets> cornerAssets = new();
        List<Assets> wallAssets = new();
        List<Assets> centerAssets = new();
        
        foreach (Assets asset in assets)
        {
            switch (asset.PreferencePlacement)
            {
                case AssetsPreference.Corner:
                    cornerAssets.Add(asset);
                    _remainingAssets += asset.MaxCount;
                    break;
                case AssetsPreference.Wall:
                    wallAssets.Add(asset);
                    _remainingAssets += asset.MaxCount;
                    break;
                case AssetsPreference.Center:
                    centerAssets.Add(asset);
                    _remainingAssets += asset.MaxCount;
                    break;
            }
        }

        int iterations = 0;
        
        
        while(_remainingAssets > 0 && iterations < maxIterations)
        {
            
            if (cornerAssets.Count > 0)
                DisposePerPoint(cornerAssets, _corners, true);
            
            if (wallAssets.Count > 0)
                DisposePerPoint(wallAssets, _walls, true);
            
            if (centerAssets.Count > 0)
                DisposePerPoint(centerAssets, _center, false);
            
            iterations++;
        }
    }

    /// <summary>
    /// Verify all positions on the dictionary that has the same placement preference as the asset list used. If it has directions,
    /// it checks every position that could block the asset. Else, just place it randomly.
    /// </summary>
    /// <param name="assetList"> List of assets to dispose </param>
    /// <param name="positionDict"> Positions to place those assets </param>
    /// <param name="hasDirection"> If has a direction(if isn't a center piece) </param>
    private void DisposePerPoint(List<Assets> assetList, Dictionary<Vector3, Vector3> positionDict, bool hasDirection)
    {
        if (assetList == null || assetList.Count == 0) return;
        
        List<Vector3> positionsList = new List<Vector3>(positionDict.Keys);

        // Fisher-Yates shuffle, was recomended by ClaudAI.
        for (int i = 0; i < positionsList.Count; i++)
        {
            int randomIndex = rnd.Next(i, positionsList.Count);
            (positionsList[i], positionsList[randomIndex]) = 
                (positionsList[randomIndex], positionsList[i]);
        }
        
        foreach (Vector3 position in positionsList)
        {
            if (assetList.Count == 0) break;
            if (IsUnavailable(position)) continue;
            if (_positions.TryGetValue(position, out bool occupied) && occupied) continue;
            if (rnd.NextDouble() > chanceToSpawnAsset) continue;
            
            Assets toDispose = DisposeAsset(assetList);
            if (toDispose == null) continue;
            
            if (hasDirection)
            {
                Vector3 dir = positionDict[position];
                List<Vector3> blockedPositions = new List<Vector3>();
                bool canPlace = true;

                int stepsX = Mathf.Max(1, (int)toDispose.SizePerStep.x);
                int stepsZ = Mathf.Max(1, (int)toDispose.SizePerStep.z);

                for (int a = 0; a < stepsX+1; a++)
                {
                    for (int b = 0; b < stepsZ+1; b++)
                    {
                        Vector3 blockPos = position + new Vector3(
                            a * dir.x * stepSize,
                            0,
                            b * dir.z * stepSize);

                        if (!_positions.TryGetValue(blockPos, out bool isOccupied) || isOccupied)
                        {
                            canPlace = false;
                            break;
                        }

                        blockedPositions.Add(blockPos);
                    }
                    if (!canPlace) break;
                }
                    
                if (canPlace && blockedPositions.Count > 0)
                {
                    Vector3 spawnPos = Vector3.zero;
                    foreach (Vector3 p in blockedPositions) spawnPos += p;
                    spawnPos /= blockedPositions.Count;
                    spawnPos.y = position.y + toDispose.SizePerStep.y;

                    Instantiate(toDispose.Prefab, spawnPos, Quaternion.LookRotation(positionDict[position]), allMeshes.transform);
                    
                    foreach (Vector3 p in blockedPositions)
                        _positions[p] = true;
                }
                else
                {
                    toDispose.RemainingCount++;
                    _remainingAssets++;
                    if (!assetList.Contains(toDispose))
                        assetList.Add(toDispose);
                }
            }
            else
            {
                Instantiate(toDispose.Prefab, position, Quaternion.identity, allMeshes.transform);
                _positions[position] = true;
            }
        }
    }

    /// <summary>
    /// To try dispose an asset on the designed position.
    /// </summary>
    /// <param name="thoseAssets"> List of all possible assets to place there </param>
    /// <returns> Returns the asset to dispose </returns>
    private Assets DisposeAsset(List<Assets> thoseAssets)
    {
        if (thoseAssets == null || thoseAssets.Count == 0)
            return null;

        List<Assets> availableAssets = thoseAssets
                                        .FindAll(a => a.RemainingCount > 0);
        if (availableAssets.Count == 0)
        {
            thoseAssets.Clear();
            return null;            
        }


        Assets result = availableAssets[rnd.Next(0, availableAssets.Count)];
        result.RemainingCount--;
        _remainingAssets--;

        return result;
    }

    /// <summary>
    /// For debugging. To see the points spawn on the room.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_points != null)
        {
            Gizmos.color = Color.green;
            foreach (Vector3 pos in _points)
            {
                if (_unavailablePoints.Contains(pos)) continue;
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }
    }
}
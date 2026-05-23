using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class AssetsGenerator : MonoBehaviour 
{
    [SerializeField] private List<Assets> assets;
    [SerializeField] private Collider roomArea;
    [SerializeField] private float stepSize;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private float chanceToSpawnAsset;
    [SerializeField] private int maxIterations;
    private System.Random rnd;
    private Bounds _roomsBounds;
    private List<Vector3> _points = new();
    private Dictionary<Vector3, Vector3> _corners = new (), _walls = new (), _center = new ();
    private Dictionary<Vector3, bool> _positions = new();
    private int _remainingAssets;

    private void Start()
    {
        rnd = new System.Random(RunManager.Seed);
        _roomsBounds = roomArea.bounds;

        foreach(Assets asset in assets)
            asset.RemainingCount = asset.MaxCount;

        _points = GetPositions();

        foreach (Vector3 point in _points)
        {
            _positions.Add(point, false);
        }

        OrganizePositions();
        DisposeAssets();
        Debug.Log($"Center positions found: {_center.Count}");
        Debug.Log($"Wall positions found: {_walls.Count}");
        Debug.Log($"Corner positions found: {_corners.Count}");
    }

    // Lista de Assets disponíveis, separados por tamanho e função, provavelmente uma classe

    // Metodo que obtem a area da sala para obter os pontos para fazer raycast

    private List<Vector3> GetPositions()
    {
        List<Vector3> positions = new ();

        for (float x = _roomsBounds.min.x; x <= _roomsBounds.max.x; x += stepSize)
        {
            for (float z = _roomsBounds.min.z; z <= _roomsBounds.max.z; z += stepSize)
            {
                Vector3 origin = new Vector3(x, _roomsBounds.max.y + 1f, z);
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
                {
                    float roundedX = Mathf.Round(hit.point.x / stepSize) * stepSize;
                    float roundedZ = Mathf.Round(hit.point.z / stepSize) * stepSize;
                    Vector3 roundedPoint = new Vector3(roundedX, hit.point.y, roundedZ);
                    positions.Add(roundedPoint);
                }
            }
        }

        return positions;
    }

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
                    if (hasXPlus && hasXMinus) _walls.Add(point, Vector3.zero);
                    else if (hasZPlus && hasZMinus) _walls.Add(point, Vector3.zero);
                    else if (hasXPlus) _walls.Add(point, Vector3.right);
                    else if (hasXMinus) _walls.Add(point, Vector3.left);
                    else if (hasZPlus) _walls.Add(point, Vector3.forward);
                    else if (hasZMinus) _walls.Add(point, Vector3.back);
                }
            }
            else if (neighborCount == 3)
            {
                if (!hasXPlus && hasXMinus && hasZPlus && hasZMinus) _walls.Add(point, Vector3.right);
                else if (!hasXMinus && hasXPlus && hasZPlus && hasZMinus) _walls.Add(point, Vector3.left);
                else if (!hasZPlus && hasXPlus && hasXMinus && hasZMinus) _walls.Add(point, Vector3.forward);
                else if (!hasZMinus && hasXPlus && hasXMinus && hasZPlus) _walls.Add(point, Vector3.back);
            }
            else if (neighborCount == 4)
            {
                _center.Add(point, Vector3.zero);
            }
        }
    }
    // Metodo para fazer raycast ao chão

    // Metodo para selecionar o objeto ideal
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

    private void DisposePerPoint(List<Assets> assetList, Dictionary<Vector3, Vector3> positionDict, bool hasDirection)
    {
        if (assetList == null || assetList.Count == 0) return;
        
        List<Vector3> positionsList = new List<Vector3>(positionDict.Keys);
        
        for (int i = 0; i < positionsList.Count; i++)
        {
            int randomIndex = rnd.Next(i, positionsList.Count);
            Vector3 temp = positionsList[i];
            positionsList[i] = positionsList[randomIndex];
            positionsList[randomIndex] = temp;
        }
        
        foreach (Vector3 position in positionsList)
        {
            if (assetList.Count == 0) break;
            
            if (_positions[position]) continue;
            
            if (rnd.NextDouble() > chanceToSpawnAsset) continue;
            
            Assets toDispose = DisposeAsset(assetList);
            if (toDispose == null) continue;
            
            if (hasDirection)
            {
                Vector3 destination = position + new Vector3(
                    toDispose.SizePerStep.x * positionDict[position].z, 
                    toDispose.SizePerStep.y, 
                    toDispose.SizePerStep.z * positionDict[position].x);
                    
                if (_positions.ContainsKey(destination) && !_positions[destination])
                {
                    Instantiate(toDispose.Prefab, destination, Quaternion.LookRotation(positionDict[position]));
                    _positions[position] = true;
                    _positions[destination] = true;
                }
                else
                {
                    toDispose.RemainingCount++;
                    _remainingAssets++;
                    assetList.Add(toDispose);
                }
            }
            else
            {
                if (!_positions[position])
                {
                    Instantiate(toDispose.Prefab, position, Quaternion.identity);
                    _positions[position] = true;
                }
            }
        }
    }

    private Assets DisposeAsset(List<Assets> thoseAssets)
    {
        if (thoseAssets == null || thoseAssets.Count == 0)
            return null;

        List<Assets> availableAssets = thoseAssets.FindAll(a => a.RemainingCount > 0);
        if (availableAssets.Count == 0) return null;

        Assets result = availableAssets[rnd.Next(0, availableAssets.Count)];
        result.RemainingCount--;
        _remainingAssets--;

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        if (_points != null)
        {
            Gizmos.color = Color.green;
            foreach (Vector3 pos in _points)
            {
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }
    }
}
using System.Collections.Generic;
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
        List<Assets> cornerAssets = new ();
        List<Assets> wallAssets = new ();
        List<Assets> centerAssets = new ();
        
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
            DisposePerPoint(cornerAssets, wallAssets, centerAssets);
            iterations++;
        }
    }

    private void DisposePerPoint(List<Assets> cornerAssets, List<Assets> wallAssets, List<Assets> centerAssets)
    {
        List<Vector3> positionsList = new (_positions.Keys);
        //if (rnd.NextDouble() > chanceToSpawnAsset) return;

        foreach (Vector3 position in positionsList)
        {
            if (_positions[position]) continue;

            if(_corners.ContainsKey(position))
            {
                Assets toDispose = DisposeAsset(cornerAssets);
                if (toDispose == null) continue;
                Vector3 destination = position + new Vector3(toDispose.SizePerStep.x * _corners[position].x, toDispose.SizePerStep.y, toDispose.SizePerStep.z * _corners[position].z);
                if (_positions.ContainsKey(destination) == false) continue;
                if (_positions[destination] == false)
                {
                    Instantiate(toDispose.Prefab, position, Quaternion.identity);
                    _positions[position] = true;
                    _positions[destination] = true;
                }
            }
            else if(_walls.ContainsKey(position))
            {
                Assets toDispose = DisposeAsset(wallAssets);
                if (toDispose == null) continue;
                Vector3 destination = position + new Vector3(toDispose.SizePerStep.x * _walls[position].x, toDispose.SizePerStep.y, toDispose.SizePerStep.z * _walls[position].z);
                if (_positions.ContainsKey(destination) == false) continue;
                if (_positions[destination] == false)
                {
                    Instantiate(toDispose.Prefab, position, Quaternion.identity);
                    _positions[position] = true;
                    _positions[destination] = true;
                }
            }
            else if(_center.ContainsKey(position))
            {
                Assets toDispose = DisposeAsset(centerAssets);
                if (toDispose == null) continue;

                if (_positions[position] == false)
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

        Assets result;

        result = thoseAssets[rnd.Next(0,thoseAssets.Count)];
        result.RemainingCount--;
        _remainingAssets--;

        if(result.RemainingCount < 1)
            thoseAssets.Remove(result);

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        if (_points != null)
        {
            Gizmos.color = Color.green;
            foreach (var pos in _points)
            {
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }
    }
}
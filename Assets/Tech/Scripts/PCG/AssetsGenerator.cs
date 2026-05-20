using System.Collections.Generic;
using UnityEngine;

public class AssetsGenerator : MonoBehaviour 
{
    [SerializeField] private List<Assets> assets;
    [SerializeField] private Collider roomArea;
    [SerializeField] private float stepSize;
    [SerializeField] private LayerMask floorLayer;
    private System.Random rnd;
    private Bounds _roomsBounds;
    private List<Vector3> _points = new();
    private Dictionary<Vector3, Vector3> _corners = new (), _walls = new (), _center = new ();
    private Dictionary<Vector3, bool> _positions = new();
    private int _actualAssetCount;
    private Assets _lastAsset;

    private void Start()
    {
        rnd = new System.Random(RunManager.Seed);
        _roomsBounds = roomArea.bounds;

        _points = GetPositions();

        foreach (Vector3 point in _points)
        {
            _positions.Add(point, false);
        }

        OrganizePositions();
        DisposeAssets();
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
                    positions.Add(hit.point);
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
                if (hasXPlus && hasZPlus) _corners.Add(point, new Vector3(1,0,1));
                else if (hasXPlus && hasZMinus) _corners.Add(point, new Vector3(1,0,-1));
                else if (hasXMinus && hasZPlus) _corners.Add(point, new Vector3(-1,0,1));
                else if (hasXMinus && hasZMinus) _corners.Add(point, new Vector3(-1,0,-1));
            }
            else if (neighborCount == 3)
                if (hasXPlus && hasXMinus && hasZPlus) _walls.Add(point, Vector3.forward);
                else if (hasXPlus && hasXMinus && hasZMinus) _walls.Add(point, Vector3.back);
                else if (hasZPlus && hasZMinus && hasXPlus) _walls.Add(point, Vector3.right);
                else if (hasZPlus && hasZMinus && hasXMinus) _walls.Add(point, Vector3.left);
            else
                _center.Add(point, Vector3.zero);
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
                    break;
                case AssetsPreference.Wall:
                    wallAssets.Add(asset);
                    break;
                case AssetsPreference.Center:
                    centerAssets.Add(asset);
                    break;
            }
        }

        if (cornerAssets.Count > 0)
        {
            foreach (KeyValuePair<Vector3,Vector3> position in _corners)
            {
                Assets toDispose = DisposeAsset(cornerAssets);
                Vector3 destination = position.Key + new Vector3(toDispose.SizePerStep.x * position.Value.x, toDispose.SizePerStep.y, toDispose.SizePerStep.z * position.Value.z);

                if (_positions[destination] == false)
                {
                    Instantiate(toDispose.Prefab, position.Key, Quaternion.identity);
                    _positions[position.Key] = true;
                    _positions[destination] = true;
                }
            }
        }

        if (wallAssets.Count > 0)
        {
            foreach (KeyValuePair<Vector3,Vector3> position in _walls)
            {
                Assets toDispose = DisposeAsset(wallAssets);
                Vector3 destination = position.Key + new Vector3(toDispose.SizePerStep.x * position.Value.x, toDispose.SizePerStep.y, toDispose.SizePerStep.z * position.Value.z);

                if (_positions[destination] == false)
                {
                    Instantiate(toDispose.Prefab, position.Key, Quaternion.identity);
                    _positions[position.Key] = true;
                    _positions[destination] = true;
                }
            }
        }

        if (centerAssets.Count > 0)
        {
            foreach (KeyValuePair<Vector3,Vector3> position in _center)
            {
                Assets toDispose = DisposeAsset(centerAssets);
                Vector3 destination = position.Key + new Vector3(toDispose.SizePerStep.x * position.Value.x, toDispose.SizePerStep.y, toDispose.SizePerStep.z * position.Value.z);

                if (_positions[destination] == false)
                {
                    Instantiate(toDispose.Prefab, position.Key, Quaternion.identity);
                    _positions[destination] = true;
                }
            }
        }
    }

    private Assets DisposeAsset(List<Assets> thoseAssets)
    {
        Assets result;

        result = thoseAssets[rnd.Next(0,thoseAssets.Count)];
        if (result != _lastAsset) _actualAssetCount = result.MaxCount;
        _actualAssetCount--;

        if(_actualAssetCount < 1)
            thoseAssets.Remove(result);

        _lastAsset = result;
        return result;
    }
}
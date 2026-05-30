using UnityEngine;

[System.Serializable]
public class Assets
{
    public AssetsType Category = AssetsType.Decoration;
    public GameObject Prefab;
    public int MaxCount = 1;
    public int RemainingCount = 1;
    public Vector3 SizePerStep = new (1,0,1);
    public AssetsPreference PreferencePlacement = AssetsPreference.Center;
}
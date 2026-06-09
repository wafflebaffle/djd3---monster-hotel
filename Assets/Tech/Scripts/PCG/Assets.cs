using UnityEngine;

/// <summary>
/// A class to set up assets directly on UnityEditor using System.Serializable.
/// All variables are public since they need to be accessed by Assets Generator.cs. 
/// </summary>
[System.Serializable]
public class Assets
{
    public GameObject Prefab;
    public int MaxCount = 1;
    public int RemainingCount = 1;
    public Vector3 SizePerStep = new (1,0,1);
    public AssetsPreference PreferencePlacement = AssetsPreference.Center;
}
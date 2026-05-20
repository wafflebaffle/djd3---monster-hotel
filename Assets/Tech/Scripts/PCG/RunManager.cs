using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static int Seed {  get; private set; }

    [SerializeField] private bool randomSeed = true;

    private void Awake()
    {
        if (randomSeed)
        {
            Seed = DateTime.Now.GetHashCode();
        }

        Debug.Log("RUN SEED: " +  Seed);
    }
}

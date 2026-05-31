using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static int Seed {  get; private set; }

    [SerializeField] private bool randomSeed = true;
    [SerializeField] private int seed;
    [SerializeField] private FloorGenerator floorGenerator;

    private RunData runData;

    private void Start()
    {
        runData = new RunData(Seed);
        floorGenerator.GenerateFloor();
    }

    private void Awake()
    {
        if (randomSeed)
        {
            Seed = DateTime.Now.GetHashCode();
        }
        else Seed = seed;

        Debug.Log("RUN SEED: " +  Seed);
    }
}

using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static int Seed {  get; private set; }

    [SerializeField] private bool randomSeed = true;
    [SerializeField] private int seed;
    [SerializeField] private FloorGenerator floorGenerator;

    private RunData currentRun;

    private void Start()
    {
        StartRun();
    }

    private void StartRun()
    {
        int runSeed = randomSeed? DateTime.Now.GetHashCode() : seed;

        currentRun = new RunData(runSeed);

        GenerateCurrentLevel();
    }

    private void GenerateCurrentLevel()
    {
        int LevelSeed = currentRun.Seed + currentRun.CurrentLevel;

        floorGenerator.Generate(LevelSeed);
    }

    private void EndRun()
    {
        floorGenerator.ClearLevel();
    }
}

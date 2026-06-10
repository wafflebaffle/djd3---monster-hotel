using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static int Seed {  get; private set; }

    [SerializeField] private bool randomSeed = true;
    [SerializeField] private int seed;
    [SerializeField] private FloorGenerator floorGenerator;
    [SerializeField] private int maxLevels;
    [SerializeField] private string finalScene;

    private RunData currentRun;

    public int CurrentLevel => currentRun.CurrentLevel;
    public int MaxLevels => maxLevels;

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

    public void NextLevel()
    {
        if (currentRun.CurrentLevel >= maxLevels)
        {
            EndGame();
            return;
        }
        floorGenerator.ClearLevel();
        currentRun.NextLevel();
        GenerateCurrentLevel();
    }

    public void EndGame()
    {
        EndRun();
        UnityEngine.SceneManagement.SceneManager.LoadScene(finalScene);
    }

    private void GenerateCurrentLevel()
    {
        floorGenerator.Generate(currentRun.Random);
    }

    private void EndRun()
    {
        floorGenerator.ClearLevel();
    }
}

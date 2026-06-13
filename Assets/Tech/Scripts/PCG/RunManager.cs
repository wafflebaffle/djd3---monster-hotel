using System;
using UnityEngine;

/// <summary>
/// Manages the execution and progression of game runs, including level generation, advancement, and game state
/// persistence.
/// </summary>
/// <remarks>Integrates with FloorGenerator to create procedural levels and SaveManager to handle saving and
/// loading of run data. Supports both random and fixed seeds for reproducible runs.</remarks>
public class RunManager : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Current game seed shared across all systems.
    /// </summary>
    public static int Seed {  get; private set; }

    /// <summary>
    /// Bool to determine if random seed will be used.
    /// </summary>
    [SerializeField] private bool randomSeed = true;
    /// <summary>
    /// Represents the seed value used for random number generation.
    /// </summary>
    [SerializeField] private int seed;
    /// <summary>
    /// Reference to the component responsible for generating the floor layout.
    /// </summary>
    /// <remarks>Used to configure and control procedural floor generation within the level.</remarks>
    [SerializeField] private FloorGenerator floorGenerator;
    /// <summary>
    /// Interger that limits the amout of levels that will be played before the end game.
    /// </summary>
    [SerializeField] private int maxLevels;
    /// <summary>
    /// Represents the name of the final scene to be loaded.
    /// </summary>
    [SerializeField] private string finalScene;

    private RunData currentRun;
    private SaveManager saveManager;

    public int CurrentLevel => currentRun.CurrentLevel;
    public int MaxLevels => maxLevels;

    private void Awake()
    {
        saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
            saveManager.RegisterSaveable(this);
        else
            Debug.LogError("SaveManager not found in scene!");
    }
    private void Start()
    {
        Debug.Log("This exists?: " + PlayerPrefs.GetInt("LoadExistingSave"));
        if (PlayerPrefs.HasKey("LoadExistingSave") && PlayerPrefs.GetInt("LoadExistingSave") == 1)
        {
            PlayerPrefs.DeleteKey("LoadExistingSave");
            if (saveManager != null && saveManager.HasSavedGame())
                saveManager.LoadGame();
            else
                StartRun();
        }
        else
        {
            StartRun();
        }
    }

    private void StartRun()
    {
        int runSeed;
        if (PlayerPrefs.HasKey("RunSeed"))
        {
            runSeed = PlayerPrefs.GetInt("RunSeed");
            PlayerPrefs.DeleteKey("RunSeed");
        }
        else
        {
            runSeed = randomSeed ? System.DateTime.Now.GetHashCode() : seed;
        }
        seed = runSeed;
        Seed = seed;
        currentRun = new RunData(Seed);
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

        if (saveManager != null)
            saveManager.SaveGame();
    }

    public void EndGame()
    {
        EndRun();
        UnityEngine.SceneManagement.SceneManager.LoadScene(finalScene);
    }

    private void GenerateCurrentLevel()
    {
        floorGenerator.Generate(currentRun.GetLevelRandom());
    }

    private void EndRun()
    {
        floorGenerator.ClearLevel();
    }

    public string GetSaveID() => "RunManager";

    public string GetSaveDataAsJson()
    {
        SaveData data = new SaveData { seed = currentRun.Seed, level = currentRun.CurrentLevel };
        return JsonUtility.ToJson(data);
    }

    public void LoadFromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("LoadFromJson: JSON string is empty or null.");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data.seed == 0)
        {
            Debug.LogError("LoadFromJson: Failed to deserialize RunData from JSON.");
            return;
        }

        Seed = data.seed;

        if (floorGenerator == null)
        {
            Debug.LogError("LoadFromJson: FloorGenerator is not assigned in the Inspector!");
            return;
        }

        currentRun = new RunData(Seed, data.level);

        floorGenerator.ClearLevel();
        GenerateCurrentLevel();
    }

    [System.Serializable]
    private struct SaveData
    {
        public int seed;
        public int level;
    }
}

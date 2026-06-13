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

    /// <summary>
    /// Stores the data for the current run.
    /// </summary>
    private RunData currentRun;
    /// <summary>
    /// Manages the saving and loading of game data.
    /// </summary>
    private SaveManager saveManager;

    /// <summary>
    /// Gets the current level of the run.
    /// </summary>
    public int CurrentLevel => currentRun.CurrentLevel;
    /// <summary>
    /// Gets the maximum number of levels.
    /// </summary>
    public int MaxLevels => maxLevels;

    private void Awake()
    {
        saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
        {
            //Savemanager gets notified this object can be saved.
            saveManager.RegisterSaveable(this);
        }
        else
            Debug.LogError("SaveManager not found in scene!");
    }

    private void Start()
    {
        Debug.Log("This exists?: " + PlayerPrefs.GetInt("LoadExistingSave"));
        if (PlayerPrefs.HasKey("LoadExistingSave") && PlayerPrefs.GetInt("LoadExistingSave") == 1)
        {
            // Consume the flag so it dosent presist across further scene loads.
            PlayerPrefs.DeleteKey("LoadExistingSave");
            if (saveManager != null && saveManager.HasSavedGame())
            {
                // Restore the saved run.
                saveManager.LoadGame();
            }
            else
            {
                // No valid save, start fresh.
                StartRun();
            }
        }
        else
        {
            // Normal fresh start.
            StartRun(); 
        }
    }

    /// <summary>
    /// Initializes a new run by setting the seed value from player preferences or generating one based on the current
    /// time, then creates the run data and generates the current level.
    /// </summary>
    private void StartRun()
    {
        int runSeed;
        if (PlayerPrefs.HasKey("RunSeed"))
        {
            // Useing a seed passed from main menu.
            runSeed = PlayerPrefs.GetInt("RunSeed");
            PlayerPrefs.DeleteKey("RunSeed");
        }
        else
        {
            // Generate a random seed from current time, or use the fixed serialized seed.
            runSeed = randomSeed ? System.DateTime.Now.GetHashCode() : seed;
        }

        // Syncing inspector.
        seed = runSeed;

        // Making it global
        Seed = seed;

        // Start at level 1 by ommission
        currentRun = new RunData(Seed);
        GenerateCurrentLevel();
    }

    /// <summary>
    /// Advances to the next level by clearing the current level, updating the run state, generating the new level, and
    /// saving the game if a save manager is available.
    /// </summary>
    /// <remarks>Ends the game if the maximum number of levels is reached.</remarks>
    public void NextLevel()
    {
        // If max levels is achieved end game.
        if (currentRun.CurrentLevel >= maxLevels)
        {
            EndGame();
            return;
        }

        // Remove current level before building next one.
        floorGenerator.ClearLevel();
        currentRun.NextLevel();
        GenerateCurrentLevel();

        // Save game after every level
        if (saveManager != null)
            saveManager.SaveGame();
    }

    /// <summary>
    /// Ends the current run and transitions to the final scene.
    /// </summary>
    public void EndGame()
    {
        EndRun();
        UnityEngine.SceneManagement.SceneManager.LoadScene(finalScene);
    }

    /// <summary>
    /// Generates the current level using a random value from the current run.
    /// </summary>
    private void GenerateCurrentLevel()
    {
        floorGenerator.Generate(currentRun.GetLevelRandom());
    }

    /// <summary>
    /// Ends the current run and clears the generated floor.
    /// </summary>
    private void EndRun()
    {
        floorGenerator.ClearLevel();
    }

    /// <summary>
    /// Gets the identifier used for saving RunManager data.
    /// </summary>
    /// <returns>The string "RunManager".</returns>
    public string GetSaveID() => "RunManager";

    /// <summary>
    /// Serializes the current run's save data to a JSON string.
    /// </summary>
    /// <returns>A JSON string representing the current save data.</returns>
    public string GetSaveDataAsJson()
    {
        SaveData data = new SaveData { seed = currentRun.Seed, level = currentRun.CurrentLevel };
        return JsonUtility.ToJson(data);
    }

    /// <summary>
    /// Loads game data from a JSON string and initializes the current run.
    /// </summary>
    /// <param name="json">The JSON string containing the game data to load.</param>
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

    /// <summary>
    /// Represents serializable data for saving the game state, including the random seed and current level.
    /// </summary>
    [System.Serializable]
    private struct SaveData
    {
        public int seed;
        public int level;
    }
}

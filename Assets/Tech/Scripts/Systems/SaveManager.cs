using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Save manager of teaher Nelio with small alterations
/// </summary>
public class SaveManager : MonoBehaviour
{
    /// <summary>
    /// Filename used for saving the game data.
    /// </summary>
    [SerializeField] private string _saveFilename = "savegame.json";
    /// <summary>
    /// Indicates whether to format the output with indentation and line breaks.
    /// </summary>
    [SerializeField] private bool   _prettyPrint;

    /// <summary>
    /// Stores the file path used for saving data.
    /// </summary>
    private string                          _saveFilePath;
    /// <summary>
    /// Stores saveable objects mapped to their string identifiers.
    /// </summary>
    private Dictionary<string, ISaveable>   _saveables;

    void Awake()
    {
        _saveables = new Dictionary<string, ISaveable>();
        _saveFilePath = Path.Combine(Application.persistentDataPath, _saveFilename);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Registers an ISaveable object for persistence, updating the existing entry if the identifier already exists.
    /// </summary>
    /// <param name="saveable">The ISaveable object to register.</param>
    public void RegisterSaveable(ISaveable saveable)
    {
        string id = saveable.GetSaveID();
        if (_saveables.ContainsKey(id))
            _saveables[id] = saveable; 
        else
            _saveables.Add(id, saveable);
    }

    /// <summary>
    /// Retrieves the saveable object associated with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the saveable object to retrieve.</param>
    /// <returns>The saveable object corresponding to the specified identifier.</returns>
    public ISaveable GetSaveable(string id)
    {
        return _saveables[id];
    }

    /// <summary>
    /// Saves the current game state to a file in JSON format.
    /// </summary>
    /// <remarks>Serializes all saveable items and writes them to the specified save file path.</remarks>
    public void SaveGame()
    {
        GameSave gameSave = new GameSave();
        gameSave.saveItems = new List<SaveItem>();

        foreach (var kvp in _saveables)
        {
            SaveItem item = new SaveItem();

            string jsonData = kvp.Value.GetSaveDataAsJson();
            if (!string.IsNullOrEmpty(jsonData))
            {
                item.id = kvp.Key;
                item.dataJson = jsonData;

                gameSave.saveItems.Add(item);
            }
        }
        
        string json = JsonUtility.ToJson(gameSave, _prettyPrint);
        File.WriteAllText(_saveFilePath, json);
        Debug.Log($"Game saved. {gameSave.saveItems.Count} items.");
    }

    /// <summary>
    /// Loads the game state from the save file if it exists.
    /// </summary>
    /// <remarks>Logs debug messages if the save file is missing or corrupted. Processes each saved item and
    /// restores its state if a matching saveable is registered.</remarks>
    public void LoadGame()
    {
        if (!File.Exists(_saveFilePath))
        {
            Debug.Log("No save file found.");
            return;
        }
        string json = File.ReadAllText(_saveFilePath);
        GameSave gameSave = JsonUtility.FromJson<GameSave>(json);
        if (gameSave?.saveItems == null)
        {
            Debug.LogError("Save file is corrupted.");
            return;
        }
        foreach (SaveItem item in gameSave.saveItems)
        {
            if (_saveables.TryGetValue(item.id, out ISaveable saveable))
                saveable.LoadFromJson(item.dataJson);
            else
                Debug.LogWarning($"No saveable registered with id '{item.id}'");
        }
        Debug.Log($"Game loaded. {gameSave.saveItems.Count} items processed.");
    }

    /// <summary>
    /// Determines whether a saved game file is present at the configured file path.
    /// </summary>
    /// <returns>true if the saved game file exists; otherwise, false.</returns>
    public bool HasSavedGame()
    {
        return File.Exists(_saveFilePath);
    }
    
    /// <summary>
    /// Deletes the save file at the configured file path.
    /// </summary>
    public void DeleteSave()
    {
        File.Delete(_saveFilePath);
    }

    /// <summary>
    /// Represents an item to be saved, containing an identifier and associated data in JSON format.
    /// </summary>
    [System.Serializable]
    private class SaveItem
    {
        public string id;
        public string dataJson;
    }

    [System.Serializable]
    private class GameSave
    {
        public List<SaveItem> saveItems;
    }
}
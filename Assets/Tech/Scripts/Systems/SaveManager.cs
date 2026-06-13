using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string _saveFilename = "savegame.json";
    [SerializeField] private bool   _prettyPrint;

    private string                          _saveFilePath;
    private Dictionary<string, ISaveable>   _saveables;

    void Awake()
    {
        _saveables = new Dictionary<string, ISaveable>();
        _saveFilePath = Path.Combine(Application.persistentDataPath, _saveFilename);
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        string id = saveable.GetSaveID();
        if (_saveables.ContainsKey(id))
            _saveables[id] = saveable; 
        else
            _saveables.Add(id, saveable);
    }

    public ISaveable GetSaveable(string id)
    {
        return _saveables[id];
    }

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

    public bool HasSavedGame()
    {
        return File.Exists(_saveFilePath);
    }

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
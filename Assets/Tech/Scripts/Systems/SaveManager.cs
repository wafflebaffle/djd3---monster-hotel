using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string _saveFilename;
    [SerializeField] private bool   _prettyPrint;

    private string                          _saveFilePath;
    private Dictionary<string, ISaveable>   _saveables;

    void Awake()
    {
        _saveFilePath   = Path.Combine(Application.persistentDataPath, _saveFilename);
        _saveables      = new Dictionary<string, ISaveable>();
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        _saveables.Add(saveable.GetSaveID(), saveable);
    }

    public ISaveable GetSaveable(string id)
    {
        return _saveables[id];
    }

    public void SaveGame()
    {
        GameSave gameSave;
        gameSave.saveItems = new List<SaveItem>();

        SaveItem saveItem;

        foreach (string key in _saveables.Keys)
        {
            saveItem.id     = key;
            saveItem.data   = _saveables[key].GetSaveData();

            gameSave.saveItems.Add(saveItem);
        }

        string jsonGameSave = JsonUtility.ToJson(gameSave, _prettyPrint);
        File.WriteAllText(_saveFilePath, jsonGameSave);

        print("Game saved.");
    }

    public void LoadGame()
    {
        if (File.Exists(_saveFilePath))
        {
            string jsonGameSave = File.ReadAllText(_saveFilePath);
            GameSave gameSave = JsonUtility.FromJson<GameSave>(jsonGameSave);

            foreach (SaveItem saveItem in gameSave.saveItems)
                _saveables[saveItem.id].LoadSaveData(saveItem.data);

            print("Game loaded.");
        }
        else
            print("No save file.");
    }

    [System.Serializable]
    private struct SaveItem
    {
        public string id;
        [SerializeReference]
        public object data;
    }

    [System.Serializable]
    private struct GameSave
    {
        public List<SaveItem> saveItems;
    }
}
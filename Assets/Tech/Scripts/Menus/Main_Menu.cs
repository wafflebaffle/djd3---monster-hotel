using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour
{
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject seedChoiceScreen;

    [Header("Animations")]
    [SerializeField] private Animator camAnim;
    [SerializeField] private string animSettingsName = "Settings";
    [SerializeField] private string animStartName = "Start";
    [SerializeField] private string annimRestoreSettingsName = "Restore";
    [SerializeField] private float animationTime = 60.0f;

    [Header("AmbientMusic")]
    [SerializeField] private AudioMixerGroup MusicGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip music1;

    [Header("Seed Settings")]
    [SerializeField] private TMP_InputField seedInputField;

    [Header("Continue")]
    [SerializeField] private SaveManager SaveManager;
    [SerializeField] private string saveFileName = "savegame.json";

    private void Start()
    {
        audioSource.clip = music1;
        audioSource.Play();

        if (audioSource && MusicGroup)
        {
            audioSource.outputAudioMixerGroup = MusicGroup;
        }

    }

    public void OpenSeedChoice()
    {
        startScreen.SetActive(false);
        seedChoiceScreen.SetActive(true);
    }

    public void StartRandomSeed()
    {
        int runSeed = UnityEngine.Random.Range(0, int.MaxValue);
        SeedStart(runSeed);
        seedChoiceScreen.SetActive(false);
    }
    
    public void StartWithCustomSeed()
    {
        int runSeed;
        if (seedInputField != null && !string.IsNullOrWhiteSpace(seedInputField.text) && int.TryParse(seedInputField.text, out int parsedSeed)) 
        {
            runSeed = parsedSeed;
        }
        else
        {
            runSeed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        SeedStart(runSeed);
    }

    private void SeedStart(int runSeed)
    {
        PlayerPrefs.SetInt("RunSeed", runSeed);
        PlayerPrefs.Save();
        seedChoiceScreen.SetActive(false);

        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        camAnim.SetTrigger(animStartName);
        StartCoroutine(StartAfterAnim(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void SettingsScreen()
    {
        camAnim.SetTrigger(animSettingsName);
        startScreen.SetActive(false);        
        settingsScreen.SetActive(true);
    }

    public void ExitSetings()
    {
        camAnim.SetTrigger(annimRestoreSettingsName);
        settingsScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    public void ExitSeedChoice()
    {
        seedChoiceScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator StartAfterAnim(int index)
    {
        YieldInstruction wfs = new WaitForSeconds(animationTime);
        yield return wfs;
        startScreen.SetActive(false);
        settingsScreen.SetActive(false);
        seedChoiceScreen.SetActive(false);
        SceneManager.LoadScene(index);
    }

    public void ContinueGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        if (SaveManager == null)
        {
            Debug.LogError("SaveManager not found");
            return;
        }

        int savedSeed = GetSavedSeed(savePath);
        PlayerPrefs.SetInt("RunSeed", savedSeed);
        PlayerPrefs.Save();

        SaveManager.LoadGame();

        seedChoiceScreen.SetActive(false);
        camAnim.SetTrigger(animStartName);
        StartCoroutine(StartAfterAnim(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private int GetSavedSeed(string filePath)
    {
        string json = File.ReadAllText(filePath);

        GameSave tempSave = JsonUtility.FromJson<GameSave>(json);

        foreach(var item in tempSave.saveItems)
        {
            if (item.id == "GameManager")
            {
                SeedData seedData = JsonUtility.FromJson<SeedData>(item.data.ToString());
                return seedData.seed;
            }
        }
        Debug.LogWarning("seed not found in save using 0.");
        return 0;
    }

    [System.Serializable]
    private struct GameSave { public List<SaveItem> saveItems; }
    [System.Serializable]
    private struct SaveItem { public string id; public object data; } // mantém object, mas só para leitura
    [System.Serializable]
    private struct SeedData { public int seed; }
}

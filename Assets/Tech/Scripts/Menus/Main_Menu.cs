using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main menu controller.
/// </summary>
public class Main_Menu : MonoBehaviour
{
    /// <summary>
    /// Settings panel.
    /// </summary>
    [SerializeField] private GameObject settingsScreen;
    /// <summary>
    /// Main menu panel.
    /// </summary>
    [SerializeField] private GameObject startScreen;
    /// <summary>
    /// Panel for seed selection.
    /// </summary>
    [SerializeField] private GameObject seedChoiceScreen;


    /// <summary>
    /// Animator of camera transition.
    /// </summary>
    [Header("Animations")]
    [SerializeField] private Animator camAnim;
    /// <summary>
    /// Settings animation trigger name. 
    /// </summary>
    [SerializeField] private string animSettingsName = "Settings";
    /// <summary>
    ///  Start animation trigger name.
    /// </summary>
    [SerializeField] private string animStartName = "Start";
    /// <summary>
    ///  Reset settings animation trigger name.
    /// </summary>
    [SerializeField] private string annimRestoreSettingsName = "Restore";
    /// <summary>
    /// Duration of transition.
    /// </summary>
    [SerializeField] private float animationTime = 60.0f;

    /// <summary>
    /// Audio mixer group for music.
    /// </summary>
    [Header("AmbientMusic")]
    [SerializeField] private AudioMixerGroup MusicGroup;
    /// <summary>
    /// Audio source for main menu music.
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// Music.
    /// </summary>
    [SerializeField] private AudioClip music1;

    /// <summary>
    /// Input field for typing a custom seed.
    /// </summary>
    [Header("Seed Settings")]
    [SerializeField] private TMP_InputField seedInputField;

    /// <summary>
    /// Reference to SaveManager
    /// </summary>
    [Header("Continue")]
    [SerializeField] private SaveManager SaveManager;
    /// <summary>
    /// Name of the save files
    /// </summary>
    [SerializeField] private string saveFileName = "savegame.json";

    private void Start()
    {
        if (audioSource && MusicGroup)
        {
            audioSource.outputAudioMixerGroup = MusicGroup;
            audioSource.clip = music1;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Displays the seed choice screen and hides the start screen.
    /// </summary>
    public void OpenSeedChoice()
    {
        startScreen.SetActive(false);
        seedChoiceScreen.SetActive(true);
    }

    /// <summary>
    /// Initializes a random seed for the pcg.
    /// </summary>
    public void StartRandomSeed()
    {
        int runSeed = UnityEngine.Random.Range(0, int.MaxValue);
        SeedStart(runSeed);
    }
    
    /// <summary>
    /// Initializes the process with a custom seed value based on user input.
    /// </summary>
    public void StartWithCustomSeed()
    {
        int runSeed;
        if (seedInputField != null && !string.IsNullOrWhiteSpace(seedInputField.text) && int.TryParse(seedInputField.text, out int parsedSeed))
            runSeed = parsedSeed;
        else
            runSeed = UnityEngine.Random.Range(0, int.MaxValue);
        SeedStart(runSeed);
    }

    /// <summary>
    /// Initializes a new game session with the specified random seed, resets relevant player preferences, deletes any
    /// existing save file, and transitions to the next scene after playing the start animation.
    /// </summary>
    /// <param name="runSeed">The seed value used to initialize the game session.</param>
    private void SeedStart(int runSeed)
    {
        // Store the seed in PlayerPrefs.
        PlayerPrefs.SetInt("RunSeed", runSeed);
        PlayerPrefs.DeleteKey("LoadExistingSave");
        PlayerPrefs.Save();

        // Delete any leftover save file to start fresh.
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(savePath)) File.Delete(savePath);

        seedChoiceScreen.SetActive(false);

        // Trigger animation
        camAnim.SetTrigger(animStartName);
        StartCoroutine(StartAfterAnim(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// <summary>
    /// Activates the settings screen, deactivates the start screen, and triggers the camera animation for the settings
    /// view.
    /// </summary>
    public void SettingsScreen()
    {
        camAnim.SetTrigger(animSettingsName);
        startScreen.SetActive(false);        
        settingsScreen.SetActive(true);
    }

    /// <summary>
    /// Restores camera settings and toggles visibility between the settings and start screens.
    /// </summary>
    public void ExitSetings()
    {
        camAnim.SetTrigger(annimRestoreSettingsName);
        settingsScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    /// <summary>
    /// Deactivates the seed choice screen and activates the start screen.
    /// </summary>
    public void ExitSeedChoice()
    {
        seedChoiceScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Waits for the animation duration, deactivates menu screens, and loads the specified scene.
    /// </summary>
    /// <param name="index">The build index of the scene to load.</param>
    /// <returns>An enumerator for coroutine execution.</returns>
    private IEnumerator StartAfterAnim(int index)
    {
        YieldInstruction wfs = new WaitForSeconds(animationTime);
        yield return wfs;
        startScreen.SetActive(false);
        settingsScreen.SetActive(false);
        seedChoiceScreen.SetActive(false);
        SceneManager.LoadScene(index);
    }

    /// <summary>
    /// Continues the game from the last saved state.
    /// </summary>
    /// <remarks>If no save file is found, a warning is logged.</remarks>
    public void ContinueGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        // Set the flag so the game knows to load existing data instead of starting fresh.
        PlayerPrefs.SetInt("LoadExistingSave", 1);
        PlayerPrefs.Save();

        camAnim.SetTrigger(animStartName);
        StartCoroutine(StartAfterAnim(SceneManager.GetActiveScene().buildIndex + 1));
    }
}

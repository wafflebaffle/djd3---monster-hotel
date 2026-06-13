using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the pause menu, including pausing and resuming gameplay, displaying confirmation prompts, saving progress,
/// and handling game exit operations.
/// </summary>
public class Pause_Menu : MonoBehaviour
{
    /// <summary>
    /// Represents the pause menu GameObject.
    /// </summary>
    [SerializeField] private GameObject pauseMenu;
    /// <summary>
    /// Represents the GameObject for the confirmation screen.
    /// </summary>
    [SerializeField] private GameObject areYouSure;
    /// <summary>
    /// Represents the input action for canceling an operation.
    /// </summary>
    [SerializeField] private InputAction cancel;

    /// <summary>
    /// Bool that controls if its paused
    /// </summary>
    private bool _isPaused;
    /// <summary>
    /// Access to is paused
    /// </summary>
    public bool Paused => _isPaused;

    private void OnEnable()
    {
        cancel.Enable();
    }

    private void OnDisable()
    {
        cancel.Disable();
    }


    void Update()
    {
        if (cancel.WasPressedThisFrame())
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Pauses gameplay, activates the pause menu, and halts time progression.
    /// </summary>
    public void Pause()
    {
        _isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Resumes gameplay by unpausing and restoring the time scale.
    /// </summary>
    public void Resume()
    {
        _isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

    }

    /// <summary>
    /// Displays a confirmation prompt to the user.
    /// </summary>
    public void YouSure()
    {
        areYouSure.SetActive(true);
    }

    /// <summary>
    /// Hides the areYouSure UI element.
    /// </summary>
    public void NotSure()
    {
        areYouSure.SetActive(false);
    }

    /// <summary>
    /// Exits the current game session, resumes normal time progression, cleans up the save manager, and loads the main
    /// scene.
    /// </summary>
    public void Exit()
    {
        _isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Destroy(FindFirstObjectByType<SaveManager>().gameObject);
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Saves the current game state.
    /// </summary>
    public void SaveGame()
    {
        SaveManager sm = FindFirstObjectByType<SaveManager>();
        if (sm != null) sm.SaveGame();
    }
}

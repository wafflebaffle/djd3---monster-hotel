using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death_Menu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName;
    private const string RunSeedKey = "RunSeed";

    public void Show()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetInt(RunSeedKey, RunManager.Seed);
        PlayerPrefs.Save();

        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

}

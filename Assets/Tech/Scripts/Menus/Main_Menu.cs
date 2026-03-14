using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [SerializeField] private GameObject settingsScreen;
    public void StartGame(int sceneindex)
    {
        SceneManager.LoadScene(sceneindex);
    }

    public void SettingsScreen()
    {
        settingsScreen.SetActive(true);
    }

    public void ExitSetings()
    {
        settingsScreen.SetActive(false);
    }
}

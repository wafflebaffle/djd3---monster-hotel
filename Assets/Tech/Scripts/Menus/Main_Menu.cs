using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void StartGame(int sceneindex)
    {
        SceneManager.LoadScene(sceneindex);
    }
}

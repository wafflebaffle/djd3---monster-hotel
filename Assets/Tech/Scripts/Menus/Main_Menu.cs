using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Main_Menu : MonoBehaviour
{
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject startScreen;
    [Header("Animations")]
    [SerializeField] private Animator camAnim;
    [SerializeField] private string animSettingsName = "Settings";
    [SerializeField] private string animStartName = "Start";
    [SerializeField] private string annimRestoreSettingsName = "Restore";
    [SerializeField] private float animationTime = 60.0f;
    [Header("AmbientMusic")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip music1;

    private void Start()
    {
        audioSource.clip = music1;
        audioSource.Play();
    }

    public void StartGame(int sceneindex)
    {
        camAnim.SetTrigger(animStartName);
        StartCoroutine(StartAfterAnim(sceneindex));
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

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator StartAfterAnim(int index)
    {
        YieldInstruction wfs = new WaitForSeconds(animationTime);
        yield return wfs;
        SceneManager.LoadScene(index);
    }
}

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;

    private const string MASTER_PARAM = "MasterVolume";
    private const string MASTER_KEY = "MASTER_VOLUME";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void SetMasterVolume(float volume)
    {
        ApplyVolume(MASTER_PARAM, volume);
        Save(volume);
    }

    private void ApplyVolume(string parameter, float volume)
    {
        float clamped = Mathf.Clamp(volume, 0.0001f, 1f);
        float db = Mathf.Log10(clamped) * 20f;
        mixer.SetFloat(parameter, db);
    }

    private void Save(float volume)
    {
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_KEY, 1f);
    }
    private void LoadSettings()
    {
        float volume = GetMasterVolume();
        ApplyVolume(MASTER_PARAM, volume);
    }
}

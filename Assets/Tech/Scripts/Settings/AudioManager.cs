using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;

    private const string MASTER_PARAM = "MasterVolume";
    private const string MASTER_KEY = "MASTER_VOLUME";
    private const string MUSIC_PARAM = "MusicVolume";
    private const string MUSIC_KEY = "MUSIC_VOLUME";
    private const string SFX_PARAM = "SFXVolume";
    private const string SFX_KEY = "SFX_VOLUME";

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
        Save(MASTER_KEY, volume);
    }

    public void SetMusicVolume(float volume)
    {
        ApplyVolume(MUSIC_PARAM, volume);
        Save(MUSIC_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        ApplyVolume(SFX_PARAM, volume);
        Save(SFX_KEY ,volume);
    }

    private void ApplyVolume(string parameter, float volume)
    {
        float clamped = Mathf.Clamp(volume, 0.0001f, 1f);
        float db = Mathf.Log10(clamped) * 20f;
        mixer.SetFloat(parameter, db);
    }

    private void Save(string key, float volume)
    {
        PlayerPrefs.SetFloat(key, volume);
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_KEY, 1f);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }


    private void LoadSettings()
    {
        float volume = GetMasterVolume();
        ApplyVolume(MASTER_PARAM, volume);
        ApplyVolume(MUSIC_PARAM, volume);
        ApplyVolume(SFX_PARAM, volume);
    }
}

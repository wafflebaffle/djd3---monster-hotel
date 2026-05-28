using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private const string MasterVolumeParam = "MASTER_VOLUME";

    private void Awake()
    {
        
    }

    public void SetMasterVolume(float volume)
    {
        ApplyVolume(MasterVolumeParam, volume);
        Save(MasterVolumeParam, volume);
    }

    private void ApplyVolume(string parameter, float volume)
    {
        float db = Mathf.Log10(volume) * 20f;
        mixer.SetFloat(parameter, db);
    }

    private void Save(string parameter, float volume)
    {
        PlayerPrefs.SetFloat(parameter, volume);
    }

    private void LoadSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MasterVolumeParam, 1f));
    }
}

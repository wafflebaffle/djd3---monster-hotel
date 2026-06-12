using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        InitializeSliders();
        RegisterListeneres();
    }

    private void InitializeSliders()
    {
        masterSlider.value = AudioManager.Instance.GetMasterVolume();
        MusicSlider.value = AudioManager.Instance.GetMusicVolume();
        SFXSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    private void RegisterListeneres()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        MusicSlider.onValueChanged.RemoveAllListeners();
        SFXSlider.onValueChanged.RemoveAllListeners();

        masterSlider.onValueChanged.AddListener(v => { AudioManager.Instance.SetMasterVolume(v); });
        MusicSlider.onValueChanged.AddListener(v => { AudioManager.Instance.SetMusicVolume(v); });
        SFXSlider.onValueChanged.AddListener(v => { AudioManager.Instance.SetSFXVolume(v); });
    }


}

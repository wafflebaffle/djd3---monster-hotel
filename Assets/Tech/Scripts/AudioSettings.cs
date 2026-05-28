using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider MasterSlider;

    private void Start()
    {
        InitializeSliders();
        RegisterListeneres();
    }

    private void InitializeSliders()
    {
        MasterSlider.value = audioManager.GetMasterVolume();
    }

    private void RegisterListeneres()
    {
        MasterSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
    }


}

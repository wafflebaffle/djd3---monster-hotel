using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;

    private void Start()
    {
        InitializeSliders();
        RegisterListeneres();
    }

    private void InitializeSliders()
    {
        masterSlider.value = AudioManager.Instance.GetMasterVolume();
    }

    private void RegisterListeneres()
    {
        masterSlider.onValueChanged.RemoveAllListeners();

        masterSlider.onValueChanged.AddListener(v => { AudioManager.Instance.SetMasterVolume(v); });
    }


}

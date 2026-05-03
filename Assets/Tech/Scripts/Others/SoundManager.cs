using System;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundData soundTypes;

    public void ToPlaySound(SoundType sound, AudioSource source, float volume = 1) 
    { 
        Sound.PlaySound(soundTypes, sound, source, volume);
    }
}
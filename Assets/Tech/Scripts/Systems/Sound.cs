using UnityEngine;

public class Sound
{
    public static void PlaySound(SoundData soundTypes, SoundType sound, AudioSource source = null, float volume = 1)
    {
        SoundList soundList = soundTypes.sounds[(int)sound];
        AudioClip[] clips = soundList.sounds;
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];

        if(source)
        {
            source.outputAudioMixerGroup = soundList.mixer;
            source.clip = randomClip;
            source.volume = volume * soundList.volume;
            source.Play();
        }
    }
}

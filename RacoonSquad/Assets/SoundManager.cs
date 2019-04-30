using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    AudioSource source;
    public Sound[] sounds;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if(source == null) source = gameObject.AddComponent<AudioSource>();
    }

    // Simple play sound function
    public void Play(string soundName)
    {
        AudioClip clip = GetAudioClipFromName(soundName);
        if(clip == null) return;

        source.PlayOneShot(clip);
    }
    public void Play(string soundName, float volume)
    {
        AudioClip clip = GetAudioClipFromName(soundName);
        if(clip == null) return;
        source.PlayOneShot(clip, volume);
    }
    public void Play(AudioClip clip)
    {
        source.PlayOneShot(clip, 1f);
    }
    public void Play(AudioClip clip, float volume)
    {
        source.PlayOneShot(clip, volume);
    }

    AudioClip GetAudioClipFromName(string name)
    {
        foreach(Sound s in sounds)
        {
            if(s.name == name) return s.clip;
        }
        return null;
    }
}

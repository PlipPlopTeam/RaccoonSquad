using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop = false;
        public bool unique = false;
    }

    static AudioSource source;

    public class MissingSoundException : System.Exception { public MissingSoundException(string msg) { Debug.LogError(msg); } };

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if(source == null) source = gameObject.AddComponent<AudioSource>();
    }

    // General function
    public static void Play(AudioClip clip, AudioSource source, float volume = 1f, float pitch = 1f)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }

    // Specifics
    public static void Play(string soundName, float volume=1f)
    {
        Play(GetAudioClipFromName(soundName), source, volume);
    }

    public static void PlayWithRandomPitch(string soundName, float volume = 1f)
    {
        Play(GetAudioClipFromName(soundName), source, volume, RandomPitch()); 
    }

    public static void PlayAtPosition(string soundName, Vector3 position, bool randomPitch=false, float volume = 1f)
    {
        var g = new GameObject();
        var clip = GetAudioClipFromName(soundName);
        var source = g.AddComponent<AudioSource>();
        g.AddComponent<DestroyAfter>().lifespan = clip.length + 1f;
        g.transform.position = position;

        Play(clip, source, volume, randomPitch ? RandomPitch() : 1f); 
    }

    static float RandomPitch()
    {
        return 1 - 0.05f + Random.value / 10; // +/- 0.05
    }

    static AudioClip GetAudioClipFromName(string name)
    {
        foreach(Sound s in Library.instance.sounds)
        {
            if(s.name == name) return s.clip;
        }
        throw new MissingSoundException("COULD NOT FIND SOUND NAMED [" + name + "]\nDid you type the name correctly?");
    }
}

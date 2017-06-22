using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioClip Click;
    public AudioClip Wind;
    public AudioClip Ambience;
    public AudioClip[] randomSounds;

    AudioSource[] audioPool;

    AudioSource WindSource;
    AudioSource sfxSounds;
    AudioSource ambienceSounds;

    public bool playSounds = false;
    public int audioSources = 16;

    // Use this for initialization
    void Awake()
    {
        audioPool = new AudioSource[audioSources];

        for (int i = 0; i < audioSources; i++)
        {
            audioPool[i] = gameObject.AddComponent<AudioSource>();
        }

        WindSource = gameObject.AddComponent<AudioSource>();
        sfxSounds = gameObject.AddComponent<AudioSource>();
        ambienceSounds = gameObject.AddComponent<AudioSource>();

        ambienceSounds.loop = true;
        
        ambienceSounds.clip = Ambience;

        ambienceSounds.volume = 0.2f;
        WindSource.volume = 0.6f;
        WindSource.clip = Wind;
        WindSource.loop = true;
        sfxSounds.volume = 0.1f;
    }

    void Start()
    {
        if (playSounds) ambienceSounds.Play();
    }

    void Update()
    {
        if (playSounds && Random.value < 0.002)
        {
            RandomSound();
        }

        

    }

    public void RandomSound()
    {
        float randomPitch = Random.Range(.90f, 1.1f);
        int randomIndex = Random.Range(0, randomSounds.Length);
        AudioSource source = findAudioSource();
        source.clip = randomSounds[randomIndex];
        source.volume = 0.2f;
        source.Play();
    }

    public void PlayWind()
    {
        float randomPitch = Random.Range(.90f, 1.1f);
        WindSource.pitch = randomPitch;
        WindSource.Play();
    }

    public void StopWind()
    {
        WindSource.Stop();
    }



    public void PlayClick()
    {
        float randomPitch = Random.Range(.94f, 1.06f);
        AudioSource source = findAudioSource();
        source.clip = Click;
        source.pitch = randomPitch;
        source.Play();
    }

    AudioSource findAudioSource()
    {
        AudioSource source = null;
        for (int i = 0; i < audioSources; i++)
        {
            if (!audioPool[i].isPlaying) source = audioPool[i];
        }

        return source;
    }

}
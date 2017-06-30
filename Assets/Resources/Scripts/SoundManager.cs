using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioClip Click;
    public AudioClip Wind;
    public AudioClip Ambience;
    public AudioClip[] randomSounds;

    AudioSource[] audioPool;

    AudioSource WindSource;
    AudioSource sfxSounds;
    AudioSource ambienceSounds;

    bool playSounds = true;
    bool playMusic = true;
    public int audioSources = 16;

    // Use this for initialization
    void Awake()
    {
        if(Instance != null)
            Destroy(this.gameObject);
        Instance = this;

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

        ambienceSounds.volume = 0.25f;
        WindSource.volume = 1.5f;
        sfxSounds.volume = 0.2f;
        WindSource.clip = Wind;
        WindSource.loop = true;

    }

    void Start()
    {
        if (playMusic && !ambienceSounds.isPlaying) ambienceSounds.Play();
    }

    void Update()
    {
        
        if (Random.value < 0.002 && playSounds)
        {
            RandomSound();
        }
    }

    public void RandomSound()
    {
        float randomPitch = Random.Range(.90f, 1.1f);
        int randomIndex = Random.Range(0, randomSounds.Length);
        AudioSource source = FindAudioSource();
        source.clip = randomSounds[randomIndex];
        source.volume = 0.2f;
        source.Play();
    }

    public void PlayWind()
    {
        float randomPitch = Random.Range(.90f, 1.1f);
        WindSource.pitch = randomPitch;
        if (playSounds && !WindSource.isPlaying) WindSource.Play();
    }

    public void StopWind()
    {
        WindSource.Stop();
    }

    public void PlayClick()
    {
        float randomPitch = Random.Range(.94f, 1.06f);
        AudioSource source = FindAudioSource();
        source.clip = Click;
        source.pitch = randomPitch;
        if (playSounds) source.Play();
    }

    AudioSource FindAudioSource()
    {
        AudioSource source = null;
        for (int i = 0; i < audioSources; i++)
        {
            if (!audioPool[i].isPlaying) source = audioPool[i];
        }

        return source;
    }

    public void ToggleMusic()
    {
        if (playMusic) DisableMusic();
        else EnableMusic();
    }
    void EnableMusic()
    {
        playMusic = true;
        ambienceSounds.Play();
    }
    void DisableMusic()
    {
        playMusic = false;
        ambienceSounds.Stop();
    }

}
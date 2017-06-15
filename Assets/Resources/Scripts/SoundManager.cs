using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioClip Click;
    public AudioClip Wind;
    public AudioClip Ambience;
    public AudioClip[] randomSounds;

    public bool playSounds = true;

    AudioSource InteractSounds;
    AudioSource sfxSounds;
    AudioSource ambienceSounds;

    // Use this for initialization
    void Awake()
    {
        InteractSounds = gameObject.AddComponent<AudioSource>();
        sfxSounds = gameObject.AddComponent<AudioSource>();
        ambienceSounds = gameObject.AddComponent<AudioSource>();

        ambienceSounds.loop = true;
        
        ambienceSounds.clip = Ambience;

        ambienceSounds.volume = 0.6f;
        InteractSounds.volume = 0.4f;
        sfxSounds.volume = 0.1f;
    }

    void Start()
    {
        ambienceSounds.Play();
    }

    void Update()
    {
        if (playSounds && !sfxSounds.isPlaying && Random.value < 0.001)
        {
            RandomSound();
        }
    }

    public void RandomSound()
    {
        float randomPitch = Random.Range(.90f, 1.1f);
        int randomIndex = Random.Range(0, randomSounds.Length);
        sfxSounds.clip = randomSounds[randomIndex];
        sfxSounds.Play();
    }

    public void PlayWind()
    {
        InteractSounds.pitch = 1;
        InteractSounds.clip = Wind;
        InteractSounds.Play();
    }

    public void PlayClick()
    {
        InteractSounds.clip = Click;
        float randomPitch = Random.Range(.94f, 1.06f);
        InteractSounds.pitch = randomPitch;
        InteractSounds.Play();
    }


}

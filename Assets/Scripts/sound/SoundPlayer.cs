using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AudioOwner {
    public AudioSource audioSource;
    public AudioClip audioClip;
    public GameObject owner;
}

public class SoundPlayer : MonoBehaviour
{
    // Audio players components.
	public AudioSource EffectsSource;
	public AudioSource MusicSource;

    private Dictionary<string, AudioOwner> _sfxSourceCollection = new Dictionary<string, AudioOwner>();

	// Random pitch adjustment range.
	public float LowPitchRange = .95f;
	public float HighPitchRange = 1.05f;

	// Singleton instance.
	public static SoundPlayer Instance = null;
	
	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of SoundManager, set it to this.
		if (Instance == null)
		{
			Instance = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad (gameObject);
	}

	// Play a single clip through the sound effects source.
	public void Play(AudioClip clip)
	{
		EffectsSource.clip = clip;
		EffectsSource.Play();
	}

    public void Stop()
    {
        EffectsSource.Stop();
    }

	// Play a single clip through the music source.
	public void PlayMusic(AudioClip clip)
	{
		MusicSource.clip = clip;
        
		MusicSource.Play();
	}

    public void PauseMusic()
    {
        MusicSource.Pause();
    }

    public void ContinueMusic()
    {
        MusicSource.Play();
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public void AddSFX(string name, AudioSource source, AudioClip clip, GameObject owner)
    {
        var audioOwner = new AudioOwner { audioSource = source, audioClip = clip, owner = owner };
        _sfxSourceCollection.Add(name, audioOwner);
    }

    public void RemoveSFX(string name) {
        var audioOwner = _sfxSourceCollection[name];
        Destroy(audioOwner.audioSource);
        _sfxSourceCollection.Remove(name);
    }

    public void PlaySFX(string name, bool looping = false) {
        var audioOwner = _sfxSourceCollection[name];
        
        if(looping) {
            audioOwner.audioSource.loop = true;
        }

        audioOwner.audioSource.clip = audioOwner.audioClip;

        if(looping && !audioOwner.audioSource.isPlaying)
        {
            audioOwner.audioSource.Play();
        } else if (!looping) {
            audioOwner.audioSource.Play();
        }
    }

    public void StopSFX(string name) {
        var audioOwner = _sfxSourceCollection[name];
        audioOwner.audioSource.Stop();
    }

	// Play a random clip from an array, and randomize the pitch slightly.
	public void RandomSoundEffect(params AudioClip[] clips)
	{
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

		EffectsSource.pitch = randomPitch;
		EffectsSource.clip = clips[randomIndex];
		EffectsSource.Play();
	}
}


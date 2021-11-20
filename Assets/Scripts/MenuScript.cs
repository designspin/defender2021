using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public AudioClip music;
    public AudioClip menuBtnClick;

    public AudioClip sfxVolume;
    public AudioClip sfxStart;

    private SceneTransition sceneTransition;

    void Awake() {
        sceneTransition = Camera.main.GetComponent<SceneTransition>();
    }
    
    void Start()
    {
        //Setup music source for title scene
        var source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        SoundPlayer.Instance.MusicSource = source;
        
        //Setup sfx source
        var sourceSFX = gameObject.AddComponent<AudioSource>();
        SoundPlayer.Instance.EffectsSource = sourceSFX;

        SoundPlayer.Instance.PlayMusic(music);   
    }

    public void PlayMenuSound()
    {
        SoundPlayer.Instance.Play(menuBtnClick);
    }

    public void MusicVolumeChange(System.Single vol)
    {
        SoundPlayer.Instance.MusicSource.volume = vol;
    }

    public void SFXVolumeChange(System.Single vol)
    {
        SoundPlayer.Instance.EffectsSource.volume = vol;
        SoundPlayer.Instance.Play(sfxVolume);
    }

    public void StartGame()
    {
        SoundPlayer.Instance.Play(sfxStart);
        
        SceneTransition.OnTransitionEnd += NextScene;

        sceneTransition.StartTransition();
    }

    private void NextScene()
    {
        SceneTransition.OnTransitionEnd -= NextScene;
        SceneManagerScript.Instance.LoadNextScene();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    private bool _isTransitioned = true;
    private static SceneManagerScript _instance;

    public enum SceneEnum {
        TitleScene,
        PlayScene
    }

    public static SceneManagerScript Instance {
        get {
            return _instance;
        }
    }

    public SceneEnum nextScene;

    public bool IsTransitioned {
        get {
            return _isTransitioned;
        }
        set {
            _isTransitioned = value;
        }
    }
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start()
    {
        LoadInitialScene();
    }

    private void LoadInitialScene()
    {
        nextScene = SceneEnum.TitleScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(nextScene.ToString());
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene.ToString());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string returnedScene = scene.name;

        switch(returnedScene)
        {
            case "TitleScene":
                nextScene = SceneEnum.PlayScene;
                break;
            case "PlayScene":
                nextScene = SceneEnum.TitleScene;
                GameManager.Instance.GameSceneLoaded();
                break;
        }
    }
}

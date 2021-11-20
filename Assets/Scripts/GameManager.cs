using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Prefab References
    public GameObject astronaughtPrefab;
    public GameObject alienPrefab;
    public GameObject livesIndicatorPrefab;

    private int currentWave = 0;
    private int noOfAstronauts = 10;
    private int landersToSpawn = 15;
    private int score = 0;

    private TextMeshProUGUI scoreText;
    private int lives = 3;

    private IEnumerator spawnRoutine;

    private static GameManager _instance;

    public static GameManager Instance {
        get {
            return _instance;
        }
    }

    private float _groundLevel = -0.38f;

    public float GroundLevel {
        get {
            return _groundLevel;
        }
    } 

    private GameObject _player;
    private GameObject _LevelUI;
    private GameObject _LivesPanel;

    private GameObject _Ground;

    private SceneTransition _transition;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    public void GameSceneLoaded()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _LevelUI = GameObject.FindGameObjectWithTag("LevelUI");
        _LivesPanel = GameObject.Find("LivesPanel");
        _Ground = GameObject.FindGameObjectWithTag("Ground");

        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();

        _LevelUI.SetActive(false);
        _player.SetActive(false);
        _transition = Camera.main.GetComponent<SceneTransition>();
        SceneTransition.OnTransitionEnd += GameSceneReady;
        _transition.StartTransition();

        //Setup music source for title scene
        var source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        SoundPlayer.Instance.MusicSource = source;
        
        //Setup sfx source
        var sourceSFX = gameObject.AddComponent<AudioSource>();
        SoundPlayer.Instance.EffectsSource = sourceSFX;

        AddLifeIndicators();
        UpdateScore(0);
    }

    public void GameSceneReady()
    {
        SceneTransition.OnTransitionEnd -= GameSceneReady;
        _LevelUI.SetActive(true);
        _player.SetActive(true);

        Invoke("SpawnEntities", 2f);
    }

    private void ContinueWithPlay()
    {
        Invoke("SpawnEntities", 2f);
        _player.transform.position = Vector3.zero;
        _player.SetActive(true);
    }

    private void SpawnEntities()
    {
        CreateAstronauts();
        spawnRoutine = SpawnAliens(5);
        StartCoroutine(spawnRoutine);
    }
    private void CreateAstronauts()
    {
        for(var i = 0; i < noOfAstronauts; i++) {
            var randomX = Random.Range(-3.5f,3.5f);
            var position = new Vector3(randomX, _groundLevel, 0f);
            Instantiate(astronaughtPrefab, position, Quaternion.identity);
        }
    }

    private void CreateAliens(int qty)
    {
        landersToSpawn = landersToSpawn - qty;

        for(var i = 0; i < qty; i++)
        {
            Instantiate(alienPrefab);
        }
    }

    IEnumerator SpawnAliens(int qty)
    {
        while(landersToSpawn > 0) {
            CreateAliens(qty);
            yield return new WaitForSeconds(8f);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score = score + scoreToAdd;
        scoreText.text = score.ToString();
    }

    private void AddLifeIndicators()
    {
        var x = 40f;
        var y = -40f;

        for(var i = 0; i < lives; i++) {
            var life = Instantiate(livesIndicatorPrefab); //, new Vector3(x + i * 100, y, 0), Quaternion.identity, _LivesPanel.transform);
            life.transform.SetParent(_LivesPanel.transform, false);
            life.transform.localPosition = new Vector3(x + i * 70, y, 0);
        }
        
    }

    private void LostLifeSwipe()
    {
        RemoveEntities();
        _transition.SetTransition(1);
        _transition.StartTransition();
        SceneTransition.OnTransitionEnd += LostLifeSwipeEnd;
    }

    private void LostLifeSwipeEnd()
    {
        SceneTransition.OnTransitionEnd -= LostLifeSwipeEnd;
        SceneTransition.OnTransitionEnd += ContinueWithPlay;
        
        _transition.SetTransition(2);
        _transition.StartTransition();
        _Ground.SetActive(true);
    }

    public void LostLife()
    {
        StopCoroutine(spawnRoutine);

        //remove life
        if(lives > 1) {
            lives = lives - 1;
            var lifeIndicatorsCount = _LivesPanel.transform.childCount;
            Destroy(_LivesPanel.transform.GetChild(lifeIndicatorsCount - 1).gameObject);
            LostLifeSwipe();
        } else {
            //game over
        }
    }

    private void RemoveEntities()
    {
        var Landers = GameObject.FindGameObjectsWithTag("Lander");
        var Astronauts = GameObject.FindGameObjectsWithTag("Astronaut");

        _Ground.SetActive(false);

        landersToSpawn += Landers.Length;

        for(var i = 0; i < Landers.Length; i++)
        {
            Destroy(Landers[i]);
        }

        for(var i = 0; i < Astronauts.Length; i++)
        {
            Destroy(Astronauts[i]);
        }
    }

    public void AstronautDestroyed()
    {
        noOfAstronauts = noOfAstronauts - 1;
    }

    void OnGUI()
    {
        GUI.Label(
           new Rect(
               5,                   // x, left offset
               Screen.height - 150, // y, bottom offset
               300f,                // width
               150f                 // height
           ),      
           "No of Astronaut: " + noOfAstronauts + " Landers To Spawn: " + landersToSpawn,             // the display text
           GUI.skin.textArea        // use a multi-line text area
        );
    }
}

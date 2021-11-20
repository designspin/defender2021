using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private float HorizontalMovement;
    private float VerticalMovement;
    private bool isRight = true;

    //Screenbounds
    private Vector2 screenBounds;
    private float playerWidth;
    private float playerHeight;

    //PlayerBounds
    private Vector2 _playerYBounds;

    public Vector2 PlayerYBounds {
        get {
            return _playerYBounds;
        }
    }

    private float cameraOffsetAmt = 0.6f;
    private float starFieldPos = 0;
    public CameraFollow cameraFollow;
    public Image starField;

    public Transform firePoint;
    public GameObject bulletPrefab;

    //Controls
    public Joystick joystick;
    public Button reverseBtn;
    public ButtonHandler thrustBtn;
    public ButtonHandler fireBtn;

    //AudioClips
    public GameObject explosion;
    public AudioClip explosionSFX;
    public AudioClip thrustSFX;
    public AudioClip lazerSFX;
    public bool IsRight {
        get {
            return isRight;
        }
    }

    private float nextFire = 0.5f;
    private float myTime = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SimplePool.Preload(bulletPrefab, 10);
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        SetupBoundaries();
        AddSFX();
        AddEvents();
    }

    void OnDisable()
    {
        RemoveSFX();
        RemoveEvents();
    }

    // Update is called once per frame
    void Update()
    {
        myTime = myTime + Time.deltaTime; 
        VerticalMovement = joystick.Vertical;

        if(thrustBtn.IsPressed) {
            HorizontalMovement = (isRight) ? 1 : -1;
        } else {
            HorizontalMovement = 0;
        }

        if(fireBtn.IsPressed && myTime > nextFire)
        {
            nextFire = myTime + 0.3f;
            SimplePool.Spawn(bulletPrefab, firePoint.position, transform.rotation);
            SoundPlayer.Instance.PlaySFX("LazerSFX");
            nextFire = nextFire - myTime;
            myTime = 0.0f; 
        }

        starFieldPos += rb.velocity.x * Time.deltaTime;

        starField.material.SetFloat("_VelX", starFieldPos);

        if(transform.position.x > 3.5f) {
            var dist = cameraFollow.transform.position - transform.position; 
            transform.position = new Vector3(-3.5f, transform.position.y, 0);
            cameraFollow.instantUpdate(transform.position + dist);
        }

        if(transform.position.x < -3.5f) {
            var dist = cameraFollow.transform.position - transform.position; 
            transform.position = new Vector3(3.5f, transform.position.y, 0);
            cameraFollow.instantUpdate(transform.position + dist);
        }
    }

    void SetupBoundaries()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        playerWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; 
        playerHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        _playerYBounds = new Vector2(screenBounds.y * -1 + playerHeight, screenBounds.y - playerHeight);
    }

    void AddSFX()
    {
        var thrustSource = gameObject.AddComponent<AudioSource>();
        SoundPlayer.Instance.AddSFX("ThrustSFX", thrustSource, thrustSFX, gameObject);

        var lazerSource = gameObject.AddComponent<AudioSource>();
        SoundPlayer.Instance.AddSFX("LazerSFX", lazerSource, lazerSFX, gameObject);
    }

    void AddEvents()
    {
        reverseBtn.onClick.AddListener(reverseDirection);
        thrustBtn.OnButtonDown += PlayThrustSound;
        thrustBtn.OnButtonUp += StopThrustSound;
    }

    void RemoveSFX()
    {
        SoundPlayer.Instance.RemoveSFX("ThrustSFX");
        SoundPlayer.Instance.RemoveSFX("LazerSFX");
    }

    void RemoveEvents()
    {
        reverseBtn.onClick.RemoveListener(reverseDirection);
        thrustBtn.OnButtonDown -= PlayThrustSound;
        thrustBtn.OnButtonUp -= StopThrustSound;
    }

    public void PlayThrustSound() 
    {
        SoundPlayer.Instance.PlaySFX("ThrustSFX", true);
    }

    public void StopThrustSound() 
    {
        SoundPlayer.Instance.StopSFX("ThrustSFX");
    }

    public void reverseDirection()
    {
        isRight = !isRight;
        cameraFollow.m_XOffset = (isRight) ? cameraOffsetAmt : -cameraOffsetAmt;
        transform.localScale = new Vector3(isRight ? 1 : -1, transform.localScale.y, transform.localScale.z);
    }

    void LateUpdate() {
        if(transform.position.y >=  screenBounds.y - playerHeight && rb.velocity.y > 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            VerticalMovement = 0;
        }

        if(transform.position.y <= screenBounds.y * -1 + playerHeight && rb.velocity.y < 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            VerticalMovement = 0;
        }

        Vector3 playerPos = transform.position;
        //playerPos.x = Mathf.Clamp(playerPos.x, playerPos.x + (screenBounds.x * -1) + playerWidth, playerPos.x + screenBounds.x - playerWidth);
        playerPos.y = Mathf.Clamp(playerPos.y, screenBounds.y * -1 + playerHeight, screenBounds.y - playerHeight);
        transform.position = playerPos;
    }

    void FixedUpdate() {
        if(HorizontalMovement > 0) {
            rb.velocity += Vector2.right.normalized * 1.5f * Time.deltaTime;
        }

        if(HorizontalMovement < 0) {
            rb.velocity += -Vector2.right.normalized * 1.5f * Time.deltaTime;
        }

        if(VerticalMovement > 0) {
            rb.velocity = rb.velocity + Vector2.up.normalized * Time.deltaTime;
        } else if(rb.velocity.y > 0) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        }

        if(VerticalMovement < 0) {
            rb.velocity = rb.velocity - Vector2.up.normalized * Time.deltaTime;
        } else if(rb.velocity.y < 0) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        }
    }

    void Die(Collision2D col)
    {
        gameObject.SetActive(false);
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 2.2f);
        SoundPlayer.Instance.RandomSoundEffect(explosionSFX);
        GameManager.Instance.Invoke("LostLife", 2f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag == "Lander") {
            Die(col);
        }
    }
}
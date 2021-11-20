using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
    public GameObject explosion;
    public AudioClip explosionSound;

    public AudioClip astronaughtTakenSFX;

    public Transform grabLocation;
    public bool isTargetted = false;
    private SpriteRenderer spriteRenderer;
    private float speed = 0.01f;

    private IEnumerator currentRoutine;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Move();
    }

    void Move() {
        currentRoutine = MoveTo(GetLocation());
        StartCoroutine(currentRoutine);
    }

    public void Freeze() {
        isTargetted = true;
        StopCoroutine(currentRoutine);
    }

    Vector3 GetLocation()
    {
        var distance = Random.Range(-1f, 1f);
        var destX = transform.position.x - distance;

        if(destX < transform.position.x) {
            spriteRenderer.flipX = true;
        } else {
            spriteRenderer.flipX = false;
        }

        return new Vector3(destX, transform.position.y, transform.position.z);
    }

    IEnumerator MoveTo(Vector3 dest) {

        if(dest.x > 3.5f) {
            dest.x = 3.5f;
        } 

        if(dest.x < -3.5f) {
            dest.x = -3.5f;
        }

        Vector3 from = transform.position;
        float step = (speed / (transform.position - dest).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while(t <= 1.0f) {
            t += step;
            transform.position = Vector3.Lerp(from, dest, t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = dest;
        Move();
    }

    void Die(Collision2D col)
    {
        StopCoroutine(currentRoutine);
        var parent = (transform.parent) ? transform.parent.transform.GetComponent<Alien>() : null;
        if(parent != null) {
            parent.VictimDestroyed();
        }
        
        Destroy(col.gameObject);
        gameObject.SetActive(false);
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 2.2f);
        SoundPlayer.Instance.RandomSoundEffect(explosionSound);
        Destroy(gameObject);
        GameManager.Instance.AstronautDestroyed();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag == "PlayerBullet")
        {
            Die(col);
        }

        if(col.transform.tag == "Ground")
        {
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            transform.position = new Vector3(transform.position.x, GameManager.Instance.GroundLevel, transform.position.y);
        }
    }
    
}

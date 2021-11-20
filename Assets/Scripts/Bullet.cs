using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1.5f;
    public GameObject trail;
    private Rigidbody2D rb;
    private Player player;

    void Start()
    {
        
        
    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        player = obj.GetComponent<Player>();
        rb.velocity = (player.IsRight) ? transform.right.normalized * speed : -transform.right.normalized * speed;

        if(player.IsRight) {
            rb.velocity = transform.right.normalized * speed;
            transform.localScale = new Vector3(1, 1, 1);
        } else {
            rb.velocity = -transform.right.normalized * speed;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnBecameInvisible()
    {
        SimplePool.Despawn(gameObject);
        trail.transform.localScale = new Vector3(0, trail.transform.localScale.y, trail.transform.lossyScale.z);
    }

    void Update()
    {
        if(trail.transform.localScale.x < 25) {
            trail.transform.localScale = new Vector3(trail.transform.localScale.x + speed * 35f * Time.deltaTime, trail.transform.localScale.y, trail.transform.localScale.z);
        }

        if(transform.position.x < -3.5) {
            transform.position = new Vector3(3.5f, transform.position.y, transform.position.z);
        } else if(transform.position.x > 3.5) {
            transform.position = new Vector3(-3.5f, transform.position.y, transform.position.z);
        }
    }
}

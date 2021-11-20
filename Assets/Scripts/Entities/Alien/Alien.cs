using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public GameObject explosion;
    public AudioClip explosionSound;
    public Transform[] waypoints;
    public Transform waypointTarget;

    public Astronaut astronautTarget;

    public Coroutine currentRoutine;
    public int currentWaypoint;
    public float speed = 5f;

    private AlienBaseState currentState;

    public AlienBaseState CurrentState {
        get {
            return currentState;
        }
    }

    public Animator animator;

    public readonly AlienFindMountainState FindMountainState = new AlienFindMountainState();
    public readonly AlienFollowPathState FollowPathState = new AlienFollowPathState();
    public readonly AlienGrabState GrabState = new AlienGrabState();
    public readonly AlienRetreatState RetreatState = new AlienRetreatState();
    public readonly AlienMutateState MutateState = new AlienMutateState();

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void OnEnable()
    {
        TransitionToState(FindMountainState);
    }

    void OnDisable()
    {
        if(currentState != null) {
            currentState.ExitState(this);
        }
    }

    void Update()
    {
        currentState.Update(this);
    }

    public void TransitionToState(AlienBaseState state)
    {
        if(currentState != null) {
            currentState.ExitState(this);
        }

        currentState = state;
        currentState.EnterState(this);
    }

    public int nextWaypoint() {
        if(currentWaypoint < waypoints.Length - 1) {
            currentWaypoint += 1;
        } else {
            currentWaypoint = 0;
        }

        waypointTarget = waypoints[currentWaypoint];
        
        return currentWaypoint;
    }

    public int previousWaypoint() {
        if(currentWaypoint > 0) {
            currentWaypoint -= 1;
        } else {
            currentWaypoint = waypoints.Length - 1;
        }

        waypointTarget = waypoints[currentWaypoint];
        
        return currentWaypoint;
    }

    public void VictimDestroyed()
    {
        astronautTarget = null;
        TransitionToState(FollowPathState);
    }
    
    public IEnumerator MoveTo(Vector3 dest) {
        Vector3 from = transform.position;
        float step = (speed / (transform.position - dest).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while(t <= 1.0f) {
            t += step;
            transform.position = Vector3.Lerp(from, dest, t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = dest;

        currentState.RoutineEnded(this);
    }

    public IEnumerator MoveOverTime(Vector3 dest, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while(t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, dest, t);
            yield return null;
        }

        currentState.RoutineEnded(this);
    }

    void Die(Collision2D col)
    {
        Destroy(col.gameObject);
        gameObject.SetActive(false);
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 2.2f);
        SoundPlayer.Instance.RandomSoundEffect(explosionSound);
        Destroy(gameObject);
        GameManager.Instance.UpdateScore(150);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag == "PlayerBullet")
        {
            if(currentState == RetreatState) {
                var astroRB = astronautTarget.transform.GetComponent<Rigidbody2D>();
                astronautTarget.transform.parent = null;
                astronautTarget.isTargetted = false;
                astroRB.gravityScale = 0.03f;
                astroRB.velocity = Vector2.zero;
            }

            Die(col);
        }
    }
}

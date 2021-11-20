using System;
using System.Linq;
using UnityEngine;

public class AlienFindMountainState : AlienBaseState {


    public override void EnterState(Alien alien)
    {
        float startAt = UnityEngine.Random.Range(-3.5f, 3.5f);

        var waypointsHolder = GameObject.Find("MountainPath");
        alien.waypoints = Array.FindAll(waypointsHolder.GetComponentsInChildren<Transform>(), child => child != waypointsHolder.transform);

        alien.waypointTarget = alien.waypoints.OrderBy(t => Mathf.Abs(startAt - t.position.x)).First();
        alien.currentWaypoint = Array.FindIndex(alien.waypoints, t => t == alien.waypointTarget);

        alien.transform.position = new Vector3(startAt ,0.37f ,0f);

        alien.currentRoutine = alien.StartCoroutine(alien.MoveTo(alien.waypointTarget.position));
    }

    public override void RoutineEnded(Alien alien)
    {
        alien.TransitionToState(alien.FollowPathState);
    }

    public override void Update(Alien alien)
    {

    }

    public override void ExitState(Alien alien)
    {
        if(alien.currentRoutine != null) {
            alien.StopCoroutine(alien.currentRoutine);
        }
    }
}
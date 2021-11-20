using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienFollowPathState : AlienBaseState
{
    private RaycastHit2D hit;
    private String astronautString = "Astronaut";
    private bool targetIsRight = true;

    private Transform GetClosestAstronaut(Alien alien) {
        var astronauts = GameObject.FindGameObjectsWithTag(astronautString);
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = alien.transform.position;

        foreach(GameObject target in astronauts)
        {
            Vector3 directionToTarget = target.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = target.transform;
            }
        }

        return bestTarget;
    }

    private void next(Alien alien)
    {
        var waypoint = (targetIsRight) ? alien.nextWaypoint() : alien.previousWaypoint();
        
        if(waypoint == 0) {
            alien.transform.position = new Vector3(-3.5f, alien.transform.position.y, alien.transform.position.z);
        } else if(waypoint == alien.waypoints.Length - 1) {
            alien.transform.position = new Vector3(3.5f, alien.transform.position.y, alien.transform.position.z);
        }

        alien.currentRoutine = alien.StartCoroutine(alien.MoveTo(alien.waypointTarget.position));
    }
    public override void EnterState(Alien alien)
    {
        var target = GetClosestAstronaut(alien);
        alien.animator.SetTrigger("Wonder");
        targetIsRight = (alien.transform.position.x > target.position.x) ? true : false;
        next(alien);
    }

    public override void RoutineEnded(Alien alien)
    {
        next(alien);
    }

    public override void Update(Alien alien)
    {
        
        hit = Physics2D.Raycast(alien.transform.position, -alien.transform.up, 1f, LayerMask.GetMask("Astronauts"));
        Debug.DrawRay(alien.transform.position, -alien.transform.up, Color.green, 0.1f);

        if(hit && hit.transform.CompareTag(astronautString)) {
            
            var astronaut = hit.transform.gameObject.GetComponent<Astronaut>();

            if(!astronaut.isTargetted) {
                astronaut.Freeze();
                alien.astronautTarget = astronaut;
                alien.TransitionToState(alien.GrabState);
            }
        }
    }

    public override void ExitState(Alien alien)
    {
        if(alien.currentRoutine != null) {
            alien.StopCoroutine(alien.currentRoutine);
        }
    }
}

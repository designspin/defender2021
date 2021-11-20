using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienRetreatState : AlienBaseState
{
    public override void EnterState(Alien alien)
    {
        var dest = new Vector3(alien.transform.position.x, 0.37f, alien.transform.position.y);
        alien.animator.SetTrigger("Retreat");
        alien.currentRoutine = alien.StartCoroutine(alien.MoveTo(dest));
    }

    public override void Update(Alien alien)
    {
        // if(alien.astronautTarget == null) {
        //     Debug.Log("Setting Trigger: Wonder from AlienRetreatState");
        //     alien.animator.SetTrigger("Wonder");
        //     alien.TransitionToState(alien.FollowPathState);
        // }
    }

    public override void ExitState(Alien alien)
    {
        if(alien.currentRoutine != null) {
            alien.StopCoroutine(alien.currentRoutine);
        }
    }

    public override void RoutineEnded(Alien alien)
    {
        alien.TransitionToState(alien.MutateState);
    }
}

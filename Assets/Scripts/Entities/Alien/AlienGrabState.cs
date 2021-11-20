using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienGrabState : AlienBaseState
{
    public override void EnterState(Alien alien)
    {
        alien.currentRoutine = alien.StartCoroutine(alien.MoveOverTime(alien.astronautTarget.grabLocation.position, 2f));
        alien.animator.SetTrigger("Snatch");
    }

    public override void ExitState(Alien alien)
    {
       if(alien.currentRoutine != null) {
            alien.StopCoroutine(alien.currentRoutine);
        }
    }

    public override void RoutineEnded(Alien alien)
    {
       alien.astronautTarget.transform.parent = alien.transform;
       SoundPlayer.Instance.RandomSoundEffect(alien.astronautTarget.astronaughtTakenSFX);
       alien.TransitionToState(alien.RetreatState);
    }

    public override void Update(Alien alien)
    {
       if(alien.astronautTarget == null) {
        alien.TransitionToState(alien.FollowPathState);
       }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienMutateState : AlienBaseState
{
    private string playerString = "Player";
    private GameObject target = null;

    public override void EnterState(Alien alien)
    {
        target = GameObject.FindGameObjectWithTag(playerString);
        alien.animator.SetTrigger("Mutate");
    }

    public override void Update(Alien alien)
    {
         var pos = alien.transform.position;
        // var n = alien.transform.up;
        // var ne = (alien.transform.up + alien.transform.right).normalized;
        // var e = alien.transform.right;
        // var se = (-alien.transform.up + alien.transform.right).normalized;
        // var s = -alien.transform.up;
        // var sw = (-alien.transform.up + -alien.transform.right).normalized;
        // var w = -alien.transform.right;
        // var nw = (alien.transform.up + -alien.transform.right).normalized;

        // var upHit = Physics2D.Raycast(pos, n, 1f);
        // var upRightHit = Physics2D.Raycast(pos, ne, 1f);
        // var rightHit = Physics2D.Raycast(pos, e, 1f);
        // var rightDownHit = Physics2D.Raycast(pos, se, 1f);
        // var downHit = Physics2D.Raycast(pos, s, 1f);
        // var downLeft = Physics2D.Raycast(pos, sw, 1f);
        // var leftHit = Physics2D.Raycast(pos, w, 1f);
        // var upLeftHit = Physics2D.Raycast(pos, nw, 1f);

        // Debug.DrawRay(pos, n, Color.blue, 0.1f);
        // Debug.DrawRay(pos, ne, Color.blue, 0.1f);
        // Debug.DrawRay(pos, e, Color.blue, 0.1f);
        // Debug.DrawRay(pos, se, Color.blue, 0.1f);
        // Debug.DrawRay(pos, s, Color.blue, 0.1f);
        // Debug.DrawRay(pos, sw, Color.blue, 0.1f);
        // Debug.DrawRay(pos, w, Color.blue, 0.1f);
        // Debug.DrawRay(pos, nw, Color.blue, 0.1f);

        Vector3 forward = target.transform.position - alien.transform.position;
        var right = new Vector3(forward.z, forward.y, -forward.x);
        var left = -right;

        Debug.DrawRay(pos, forward, Color.blue, 0.1f);
        Debug.DrawRay(pos, right, Color.red, 0.1f);
        Debug.DrawRay(pos, left, Color.green, 0.1f);

        float step = alien.speed * Time.deltaTime;
        alien.transform.position = Vector3.MoveTowards(alien.transform.position, target.transform.position, step);
    }

    public override void RoutineEnded(Alien alien)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(Alien alien)
    {
        throw new System.NotImplementedException();
    }
}

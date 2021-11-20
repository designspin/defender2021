using UnityEngine;

public abstract class AlienBaseState
{
    public abstract void EnterState(Alien alien);

    public abstract void Update(Alien alien);

    public abstract void ExitState(Alien alien);

    public abstract void RoutineEnded(Alien alien);
}

public enum EnemyStateType
{
    Patrol,
    Chase,
    Attack,
    Dead
}

public class EnemyStateEventData
{
    public EnemyStateType PreviousState;
    public EnemyStateType NewState;

    public EnemyStateEventData(EnemyStateType previous, EnemyStateType newState)
    {
        PreviousState = previous;
        NewState = newState;
    }
}

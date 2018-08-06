using System;
using System.Collections;

public class StateMachine
{

    private IState CurrentState;
    private IState PreviousState;

    public void ChangeState(IState newState)
    {
        if(CurrentState != null)
        { 
            CurrentState.Exit();
        }
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        if(CurrentState != null)
            CurrentState.Execute();
    }

    public void ExecuteStateFixedUpdate()
    {
        if(CurrentState != null)
            CurrentState.FixedExecute();
    }

    public void SwitchToPreviousState()
    {
        if(CurrentState != null)
            CurrentState.Exit();
        CurrentState = PreviousState;
        CurrentState.Enter();
    }

    public System.Type GetCurrentlyRunningState()
    {
        return CurrentState.GetType();
    }

    internal void ChangeState(object knockback)
    {
        throw new NotImplementedException();
    }
}

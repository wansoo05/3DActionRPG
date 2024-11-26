using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class StateMachine<T>
{
    private T sender;

    public IState<T> CurrentState { get; set; }
    public IState<T> PrevState { get; set; }

    public StateMachine(T sender, IState<T> defaultState)
    {
        this.sender = sender;
        Debug.Log(defaultState);
        SetState(defaultState);
    }

    public void SetState(IState<T> state)
    {
        if (sender == null)
            return;

        if (state == null)
            return;

        if (CurrentState == state)
            return;

        if (CurrentState != null)
        {
            CurrentState.Exit(sender, state);
        }
        PrevState = CurrentState;
        CurrentState = state;

        CurrentState.Enter(sender, PrevState);
    }

    public void Execute()
    {
        CurrentState.Execute(sender);
    }
}
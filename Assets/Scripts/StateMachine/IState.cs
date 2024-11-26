using UnityEngine;

public interface IState<T>
{
    void Enter(T sender, IState<T> prevState);
    void Execute(T sender);
    void Exit(T sender, IState<T> nextState);
}
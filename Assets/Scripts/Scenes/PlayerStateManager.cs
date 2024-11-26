using System.Collections;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public void SaveState()
    {
        IStateUpdatable[] states = GameManager.Instance.PlayerInstance.GetComponents<IStateUpdatable>();
        foreach (IStateUpdatable state in states)
            state.Save();
    }

    public void LoadState()
    {
        //���� ������ �Ѿ�� �� �÷��̾ ������ �������� ���� ���� FallingMode�� �Ǿ��ֱ� ������ �� �����Ӹ� Idle�� �ٲٰ� ������ �ε带 �����Ѵ�.
        StateComponent stateComponent = GameManager.Instance.PlayerInstance.GetComponent<StateComponent>();
        stateComponent.SetIdleMode();

        IStateUpdatable[] states = GameManager.Instance.PlayerInstance.GetComponents<IStateUpdatable>();
        foreach (IStateUpdatable state in states)
            state.Load(GameManager.Instance.PlayerState);
    }
}
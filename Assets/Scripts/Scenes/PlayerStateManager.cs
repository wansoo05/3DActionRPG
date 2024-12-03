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
        //다음 씬으로 넘어갔을 때 플레이어가 위에서 떨어지고 있을 때는 FallingMode로 되어있기 때문에 한 프레임만 Idle로 바꾸고 데이터 로드를 시작한다.
        StateComponent stateComponent = GameManager.Instance.PlayerInstance.GetComponent<StateComponent>();
        stateComponent.SetIdleMode();

        IStateUpdatable[] states = GameManager.Instance.PlayerInstance.GetComponents<IStateUpdatable>();
        foreach (IStateUpdatable state in states)
            state.Load(GameManager.Instance.PlayerState);
    }
}
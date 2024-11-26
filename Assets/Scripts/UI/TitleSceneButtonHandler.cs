using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class TitleSceneButtonHandler : MonoBehaviour
{
    [SerializeField]
    private Button[] bts;

    [SerializeField]
    private GameObject manual;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EnableButtons();
        manual.SetActive(false);
    }

    private void EnableButtons()
    {
        foreach (Button bt in bts)
            bt.enabled = true;
    }

    private void DisableButtons()
    {
        foreach (Button bt in bts)
            bt.enabled = false;
    }

    public void StartGame()
    {
        GameManager.Instance.GameStart();
        DisableButtons();
    }

    public void ShowManual()
    {
        manual.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
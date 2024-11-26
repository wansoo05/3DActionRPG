using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.PauseGame();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        GameManager.Instance.ResumeGame();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMainMenuClick()
    {
        UIController.Instance.HideMainMenuUI();
        GameManager.Instance.GameOver();
    }

    public void OnReturnClick()
    {
        UIController.Instance.HideMainMenuUI();
    }
}
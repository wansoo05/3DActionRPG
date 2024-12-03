using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGamePaused { get; private set; }
    public bool IsGameOver { get; private set; }

    private Player player;
    public Player PlayerInstance 
    {
        get
        {
            if (player == null)
                player = FindObjectOfType<Player>();
            return player;
        }
        private set { }
    }
    public InputActions input;
    /// <summary>
    /// 타임라인을 재생시켜줄 오브젝트를 찾아줌
    /// </summary>
    private TimelinePlayer timelinePlayer;
    public TimelinePlayer TimelinePlayerInstance
    {
        get
        {
            if (timelinePlayer == null)
                timelinePlayer = FindObjectOfType<TimelinePlayer>();
            return timelinePlayer;
        }
        private set { }
    }
    public PlayerStateData PlayerState;

    [HideInInspector]
    public PlayerStateManager StateManager;

    private float originTimeScale = 1.0f;
    /// <summary>
    /// 게임 시작시 최초 한번만 실행하기 위해 필요한 변수
    /// </summary>
    private bool bOnce = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        StateManager = GetComponent<PlayerStateManager>();
        input = new InputActions();

        QualitySettings.vSyncCount = 0;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive)
        {
            if (PlayerInstance != null)
            {
                if (bOnce)
                {
                    StartSetting();
                }
                else
                    StartCoroutine(LateStart());
            }
        }

    }

    /// <summary>
    /// 게임 시작 최초에 한번만 불려지는 함수
    /// </summary>
    private void StartSetting()
    {
        bOnce = false;
    }

    public void GameStart()
    {
        Time.timeScale = 1.0f;                      
        IsGamePaused = false;
        IsGameOver = false;
        bOnce = true;

        PlayerState.Initialize();
        LoadSceneManager.Instance.LoadScene("GameScene");
    }

    public void GameOver()
    {
        PauseGame();
        IsGameOver = true;
        LoadSceneManager.Instance.LoadScene("TitleScene");
    }

    public void PauseGame()
    {
        originTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = originTimeScale;
        IsGamePaused = false;
    }

    private IEnumerator LateStart()
    {
        //모든 상태들이 초기화 된 이후에 Load해준다.
        for (int i = 0; i < 2; i++)
            yield return new WaitForEndOfFrame();
        StateManager.LoadState();
    }
}
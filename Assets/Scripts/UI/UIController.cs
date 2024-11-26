using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private HUD hud;
    public HUD Hud { get { return hud; } }

    [SerializeField]
    private MainMenuUI mainMenu;

    [SerializeField]
    private BossHpBarHandler bossHpBar;

    [SerializeField]
    private MissionMessageUI missionMessage;
    public MissionMessageUI MissionMessage { get { return missionMessage; }}

    public static UIController Instance { get; private set; }

    private PlayableDirector pd;

    /// <summary>
    /// 타임라인이 플레이 되고있는지 알 수 있는 변수
    /// </summary>
    private bool bTimelinePlay;
    private Canvas missionMessageCanvas;
    
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
        missionMessageCanvas = missionMessage.GetComponent<Canvas>();
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
        if (scene.name == "LoadingScene" || scene.name == "TitleScene")
        {
            if (missionMessageCanvas.enabled)
                missionMessageCanvas.enabled = false;

            if (mainMenu.gameObject.activeSelf == true)
                HideMainMenuUI();

            if (bossHpBar.gameObject.activeSelf == true)
                HideBossHpBar();
        }

        if (mode != LoadSceneMode.Additive)
        {
            hud.Initialize();

            TimelinePlayer timelinePlayer = GameManager.Instance.TimelinePlayerInstance;

            if (timelinePlayer != null)
            {
                pd = timelinePlayer.GetComponent<PlayableDirector>();
            }

            if (pd != null)
            {
                pd.played += OnPlayed;
                pd.stopped += OnStopped;
            }
        }
    }

    private void Update()
    {
        if (bTimelinePlay == false && Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainMenu.gameObject.activeSelf == false)
            {
                ShowMainMenuUI();
            }
            else
            {
                HideMainMenuUI();
            }
        }
    }

    private void OnPlayed(PlayableDirector director)
    {
        bTimelinePlay = true;
    }

    private void OnStopped(PlayableDirector director)
    {
        bTimelinePlay = false;
    }

    #region BossHpBar

    public void ShowBossHpBar()
    {
        bossHpBar.gameObject.SetActive(true);
    }

    public void UpdateBossHpBar(float amount)
    {
        bossHpBar.UpdateHp(amount);
    }

    public void HideBossHpBar()
    {
        bossHpBar.gameObject.SetActive(false);
    }

    #endregion

    #region MainMenuUI
    public void ShowMainMenuUI()
    {
        mainMenu.gameObject.SetActive(true);
    }

    public void HideMainMenuUI()
    {
        mainMenu.gameObject.SetActive(false);
    }
    #endregion

    #region MissionUI
    public void ShowMissionUI()
    {
        missionMessageCanvas.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMissionUI()
    {
        missionMessageCanvas.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateMissionMessage(string txt)
    {
        if (missionMessageCanvas.enabled == false)
            return;
        missionMessage.NoticeMessage(txt);
    }

    public void MissionButtonSetting(MissionState state)
    {
        missionMessage.MissionButtonSetting(state);
    }
    #endregion

    #region HUD
    public void AdjustSkillCoolTime(SkillType type, float currentTime, float coolTime)
    {
        hud.AdjustSkillCoolTime(type, currentTime, coolTime);
    }

    public void UpdateBar(int type, float amount)
    {
        hud.UpdateBar(type, amount);
    }

    public void UpdateStateText(int type, string value)
    {
        hud.UpdateText(type, value);
    }

    public void UpdateMissionNotice(string txt)
    {
        hud.UpdateNotice(txt);
    }

    public void UpdateTargetingText(string txt)
    {
        hud.UpdateTargetingText(txt);
    }
    #endregion
}
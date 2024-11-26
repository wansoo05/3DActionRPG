using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthPointComponent : MonoBehaviour, IStateUpdatable
{
    [SerializeField]
    private float maxHealthPoint = 100.0f;
    private float currHealthPoint;
    private float deltaHealthPoint;

    [SerializeField]
    private string uiEnemyName = "Enemy_HealthBar";

    private Image healthImg;
    private Canvas uiEnemyCanvas;
    private CapsuleCollider capsule;
    private Coroutine damageRoutine;

    public bool Dead { get => currHealthPoint < 1.0f; }
    public bool Half { get => currHealthPoint / maxHealthPoint <= 0.5f; }

    private void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();
    }
    private void Start()
    {
        currHealthPoint = maxHealthPoint;
        deltaHealthPoint = currHealthPoint;

        if (GetComponent<AIController>() != null && GetComponent<AIContorller_Boss>() == null)
        {
            uiEnemyCanvas = UIHelpers.CreateBillBoardCanvas(uiEnemyName, transform, Camera.main);
            uiEnemyCanvas.transform.localPosition = new Vector3(0.0f, capsule.height + 0.2f, 0.0f);
            Transform t = uiEnemyCanvas.transform.FindChildByName("Image_Foreground");
            healthImg = t.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        currHealthPoint = maxHealthPoint;
        deltaHealthPoint = currHealthPoint;

        if (healthImg != null)
        {
            healthImg.fillAmount = currHealthPoint;
        }
    }

    private void Update()
    {
        if (GetComponent<Player>())
        {
            UIController.Instance.UpdateStateText(0, ((int)currHealthPoint).ToString() + " / " + ((int)maxHealthPoint).ToString());
            UIController.Instance.UpdateBar(0, deltaHealthPoint / maxHealthPoint);
        }

        if (uiEnemyCanvas == null)
            return;

        uiEnemyCanvas.transform.rotation = Camera.main.transform.rotation;

    }
    public void Damage(float amount)
    {

        currHealthPoint += amount * -1.0f;
        currHealthPoint = Mathf.Clamp(currHealthPoint, 0.0f, maxHealthPoint);
        
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
        damageRoutine = StartCoroutine(SmoothDamage(currHealthPoint, 0.5f));
    }

    public void Heal(float amount)
    {
        currHealthPoint += amount * 1.0f;
        currHealthPoint = Mathf.Clamp(currHealthPoint, 0.0f, maxHealthPoint);

        if (damageRoutine == null)
        {
            deltaHealthPoint = currHealthPoint;
        }
    }

    private IEnumerator SmoothDamage(float amount, float duration)
    {
        float elapsed = 0.0f;
        float startHealthPoint = deltaHealthPoint;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            deltaHealthPoint = Mathf.SmoothStep(startHealthPoint, amount, t);

            if (healthImg != null)
            {
                healthImg.fillAmount = deltaHealthPoint / maxHealthPoint;
            }
            else
            {
                if (GetComponent<AIContorller_Boss>() != null)
                {
                    UIController.Instance.UpdateBossHpBar(deltaHealthPoint / maxHealthPoint);
                }
            }
            yield return null;
        }

        damageRoutine = null;
    }

    public void Save()
    {
        GameManager.Instance.PlayerState.Hp = currHealthPoint;
    }

    public void Load(PlayerStateData data)
    {
        currHealthPoint = data.Hp;
        deltaHealthPoint = currHealthPoint;
    }
}
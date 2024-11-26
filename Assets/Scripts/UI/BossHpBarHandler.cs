using UnityEngine;
using UnityEngine.UI;

public class BossHpBarHandler : MonoBehaviour
{
    private Image hpBar;

    private void Awake()
    {
        Transform t = transform.FindChildByName("Image");
        hpBar = t.GetComponent<Image>();
    }

    private void OnEnable()
    {
        hpBar.fillAmount = 1.0f;
    }

    public void UpdateHp(float amount)
    {
        hpBar.fillAmount = amount;
    }
}
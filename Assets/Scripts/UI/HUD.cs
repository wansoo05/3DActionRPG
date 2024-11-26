using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Image[] ActionUIs;

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Image magicBar;

    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private TextMeshProUGUI magicText;

    [SerializeField]
    private TextMeshProUGUI missionNotice;

    [SerializeField]
    private TextMeshProUGUI targetingText;

    private Image weaponImg;
    private Image[] skillBackGround = new Image[2];
    private Image[] skillForeGround = new Image[2];
    private WeaponComponent weapon;
    private SkillComponont skill;

    //ActionUI위치 찾기 위한 Dictionary
    private Dictionary<SkillType, int> dic;


    private void Awake()
    { 
        //WeaponImageUI찾기
        weaponImg = ActionUIs[0].transform.FindChildByName("Weapon").GetComponent<Image>();
        weaponImg.enabled = false;

        //SkillImageUI찾기
        for (int i = 1; i < ActionUIs.Length; i++)
        {
            string backName = "Skill" + i.ToString() + "_Background";
            string foreName = "Skill" + i.ToString() + "_Foreground";
            skillBackGround[i - 1] = ActionUIs[i].transform.FindChildByName(backName).GetComponent<Image>();
            skillForeGround[i - 1] = skillBackGround[i - 1].transform.FindChildByName(foreName).GetComponent<Image>();
            skillForeGround[i - 1].fillAmount = 1.0f;
        }
    }

    public void Initialize()
    {
        ActionUIs[0].color = Color.blue;

        weaponImg.enabled = false;

        dic = new Dictionary<SkillType, int>();
        dic[SkillType.None] = 0;

        targetingText.text = "Targeting : Off";

        Player player = GameManager.Instance.PlayerInstance;
        if (player != null)
        {
            weapon = player.GetComponent<WeaponComponent>();
            weapon.OnWeaponTypeChanged += OnWeaponChanged;

            skill = player.GetComponent<SkillComponont>();
            skill.OnSkillRegisted += OnSkillRegisted;
            skill.OnSkillTypeChanged += OnSkillTypeChanged;
        }
    }

    private void Start()
    {
        healthBar.fillAmount = 1.0f;
        magicBar.fillAmount = 1.0f;
    }

    public void AdjustSkillCoolTime(SkillType type, float currentTime, float coolTime)
    {
        skillForeGround[dic[type] - 1].fillAmount = 1.0f - currentTime / coolTime;
    }

    public void UpdateBar(int type, float amount)
    {
        Image img = null;

        //0일 때는 HP바, 1일 때는 MP바 업데이트
        if (type == 0)
        {
            img = healthBar;
        }
        else if (type == 1)
        {
            img = magicBar;
        }

        img.fillAmount = amount;
    }

    public void UpdateText(int type, string value)
    {
        if (type == 0)
        {
            healthText.text = value;
        }
        else
        {
            magicText.text = value;
        }
    }

    public void UpdateNotice(string txt)
    {
        missionNotice.text = txt;
    }

    public void UpdateTargetingText(string txt)
    {
        targetingText.text = txt;
    }

    private void OnWeaponChanged(WeaponType type1, WeaponType type2)
    {
        Weapon playerWeapon = weapon.GetWeapon(type2);
        if (playerWeapon == null)
        {
            weaponImg.enabled = false;
            return;
        }

        //무기 아이콘 변경
        if (playerWeapon.iconImage == null)
        {
            weaponImg.enabled = false;
            return;
        }

        if (weaponImg.enabled == false)
            weaponImg.enabled = true;

        weaponImg.sprite = playerWeapon.iconImage;
    }

    private void OnSkillRegisted(SkillType type1, SkillType type2)
    {
        dic[type1] = 1;
        dic[type2] = 2;
        skillBackGround[0].sprite = skill.GetSkill(type1).iconImage;
        skillForeGround[0].sprite = skill.GetSkill(type1).iconImage;
        skillBackGround[1].sprite = skill.GetSkill(type2).iconImage;
        skillForeGround[1].sprite = skill.GetSkill(type2).iconImage;
    }

    private void OnSkillTypeChanged(SkillType type1, SkillType type2)
    {
        for (int i = 0; i < ActionUIs.Length; i++)
        {
            if (i == dic[type2])
                ActionUIs[i].color = Color.blue;
            else
                ActionUIs[i].color = Color.white;
        }
    }

}
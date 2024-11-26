using UnityEngine;
using UnityEngine.UI;

public class LoadingBarHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject LoadingBarObj;

    private Image barImg;

    private void Awake()
    {
        if (LoadingBarObj == null)
            return;

        Transform t = LoadingBarObj.transform.FindChildByName("ProgressBar");
        barImg = t.GetComponent<Image>();
    }

    private void Update()
    {
        barImg.fillAmount = LoadSceneManager.Instance.ProgressValue;
    }
}
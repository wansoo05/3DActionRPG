using System.Collections;
using UnityEngine;

public class MagicPointComponent : MonoBehaviour, IStateUpdatable
{
    [SerializeField]
    private float maxMagicPoint = 100.0f;
    [SerializeField]
    private float recoverAmount = 1.0f;

    private float currMagicPoint;
    private float deltaMagicPoint;

    private Coroutine useRoutine;

    private void OnEnable()
    {
        currMagicPoint = maxMagicPoint;
        deltaMagicPoint = currMagicPoint;
    }

    private void Update()
    {
        if (currMagicPoint < maxMagicPoint)
        {
            currMagicPoint += recoverAmount * Time.deltaTime;
        }

        currMagicPoint = Mathf.Clamp(currMagicPoint, 0, maxMagicPoint);
        if (useRoutine == null)
            deltaMagicPoint = currMagicPoint;

        UIController.Instance.UpdateBar(1, deltaMagicPoint / maxMagicPoint);
        UIController.Instance.UpdateStateText(1, ((int)currMagicPoint).ToString() + " / " + ((int)maxMagicPoint).ToString());
    }

    public bool Use(float amount)
    {
        if (currMagicPoint - amount < 0.0f)
        {
            Debug.Log("마나가 부족합니다.");
            return false;
        }

        currMagicPoint += amount * -1.0f;
        currMagicPoint = Mathf.Clamp(currMagicPoint, 0.0f, maxMagicPoint);

        if (useRoutine != null)
        {
            StopCoroutine(useRoutine);
            useRoutine = null;
        }
        useRoutine = StartCoroutine(SmoothChange(currMagicPoint, 0.5f));
        return true;
    }

    private IEnumerator SmoothChange(float amount, float duration)
    {
        float elapsed = 0.0f;
        float startMagicPoint = deltaMagicPoint;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            deltaMagicPoint = Mathf.SmoothStep(startMagicPoint, amount, t);
            yield return null;
        }
        useRoutine = null;
    }

    public void Save()
    {
        GameManager.Instance.PlayerState.Mp = currMagicPoint;
    }

    public void Load(PlayerStateData data)
    {
        currMagicPoint = data.Mp;
        deltaMagicPoint = currMagicPoint;
    }
}
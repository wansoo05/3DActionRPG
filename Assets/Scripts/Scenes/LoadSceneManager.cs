using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    private static LoadSceneManager instance;

    /// <summary>
    /// �� ��ȯ�� ������ ���൵�� ��Ÿ���� ���� ���� 0.0f ~ 1.0f ���� ��
    /// </summary>
    public float ProgressValue { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static LoadSceneManager Instance => instance;

    public void LoadScene(string name)
    {
        ProgressValue = 0.0f;
        StartCoroutine(LoadingAsync(name));
    }

    /// <summary>
    /// �񵿱� �� ��ȯ�� �ε� �������� ������ �ɾ��ִ� �ڷ�ƾ �Լ�
    /// </summary>
    /// <param name="name">�� ��ȯ�� �� ����</param>
    /// <returns></returns>
    private IEnumerator LoadingAsync(string name)
    {
        yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        string currentSceneName = SceneManager.GetActiveScene().name;
        float time1 = 0.0f;
        float time2 = 0.0f;
        float startValue = 0.0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(name,LoadSceneMode.Single);
        //�ε��� �Ϸ�ǵ� �� Ȱ��ȭ ����.
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if (operation.progress < 0.9f)
            {
                //Ÿ�� �����Ͽ� ������ ���� �ʱ� ���ؼ�
                time1 += Time.unscaledDeltaTime;
                ProgressValue = Mathf.Lerp(ProgressValue, operation.progress, time1);
                startValue = ProgressValue;
            }
            else
            {
                time2 += Time.unscaledDeltaTime;
                float t = time2 / 2.0f;
                ProgressValue = Mathf.Lerp(startValue, 1.0f, t);
            }

            //�ε��ٰ� �� ä������ �� ��ȯ
            if (ProgressValue >= 1.0f)
            {
                operation.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }
    }
}
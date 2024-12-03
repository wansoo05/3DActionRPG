using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    private static LoadSceneManager instance;

    /// <summary>
    /// 씬 전환시 임의의 진행도를 나타내기 위한 변수 0.0f ~ 1.0f 사이 값
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
    /// 비동기 씬 전환시 로딩 게이지를 딜레이 걸어주는 코루틴 함수
    /// </summary>
    /// <param name="name">씬 전환될 씬 네임</param>
    /// <returns></returns>
    private IEnumerator LoadingAsync(string name)
    {
        yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        string currentSceneName = SceneManager.GetActiveScene().name;
        float time1 = 0.0f;
        float time2 = 0.0f;
        float startValue = 0.0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(name,LoadSceneMode.Single);
        //로딩이 완료되도 씬 활성화 안함.
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if (operation.progress < 0.9f)
            {
                //타임 스케일에 영향을 받지 않기 위해서
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

            //로딩바가 다 채워지면 씬 전환
            if (ProgressValue >= 1.0f)
            {
                operation.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }
    }
}
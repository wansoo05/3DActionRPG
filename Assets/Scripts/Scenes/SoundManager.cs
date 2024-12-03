using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM = 0,
    Effect,
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

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

    public static SoundManager Instance => instance;

    [SerializeField]
    private AudioClip[] audioClips;

    private Dictionary<string, AudioClip> audioDictionary;

    private List<AudioSource> playingAudioSources;

    private void Start()
    {
        audioDictionary = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in audioClips)
        {
            audioDictionary[clip.name] = clip;
        }

        playingAudioSources = new List<AudioSource>();
    }

    private AudioClip GetAudioClip(string name)
    {
        AudioClip clip = audioDictionary[name];
        if (clip == null)
        {
            Debug.LogError(name + "이 존재하지 않음.");
        }

        return clip;
    }

    public void PlaySound(string name, SoundType type, Transform audioTarget, float volume = 1.0f)
    {
        AudioClip clip = GetAudioClip(name);
        if (clip == null)
            return;

        if (type == SoundType.BGM)
        {
            GameObject obj = new GameObject("BGM");
            AudioSource source = obj.AddComponent<AudioSource>();
            obj.transform.parent = audioTarget;

            source.clip = clip;
            playingAudioSources.Add(source);
            source.loop = true;
            source.Play();
        }
        else
        {
            GameObject obj = new GameObject("SFX");
            obj.transform.position = audioTarget.position + Vector3.up;

            AudioSource source = obj.AddComponent<AudioSource>();
            source.spatialBlend = 1.0f; //3D 사운드 지정.
            source.minDistance = 5.0f;
            source.maxDistance = 30.0f;
            source.clip = clip;
            source.rolloffMode = AudioRolloffMode.Logarithmic; //거리 증가에 따라 소리가 더 빠르게 감소

            source.volume = volume;
            source.Play();
            Destroy(obj, clip.length);
            
        }
    }

    public string SoundRandomRange(int from, int to)
    {
        if (from == 0 || to == 0)
            return "";

        int value = Random.Range(from, to + 1);

        return value < 10 ? '0' + value.ToString() : value.ToString();
    }
}
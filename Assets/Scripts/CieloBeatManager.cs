using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Se auto-inyecta en level_cielo al iniciar. Lee el AudioSource del objeto "musicManager"
/// y emite OnBeat a 172 BPM. También adjunta BeatScalePulse al objeto "obstacles".
/// </summary>
public class CieloBeatManager : MonoBehaviour
{
    public static CieloBeatManager Instance { get; private set; }
    public event Action OnBeat;

    const string SceneName = "level_cielo";
    const float BPM = 172f;

    AudioSource source;
    float secondsPerBeat;
    int lastBeat = -1;

    static bool subscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (!subscribed)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            subscribed = true;
        }
        TrySetup(SceneManager.GetActiveScene());
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => TrySetup(scene);

    static void TrySetup(Scene scene)
    {
        if (scene.name != SceneName) return;
        if (Instance != null) return;

        var musicObj = GameObject.Find("musicManager");
        if (musicObj == null)
        {
            Debug.LogWarning("[CieloBeatManager] No se encontró el objeto 'musicManager'.");
            return;
        }

        var mgr = musicObj.AddComponent<CieloBeatManager>();
        mgr.source = musicObj.GetComponent<AudioSource>();
        mgr.secondsPerBeat = 60f / BPM;
        Instance = mgr;

        var obstaclesObj = GameObject.Find("obstacles");
        if (obstaclesObj != null)
            obstaclesObj.AddComponent<BeatScalePulse>();
        else
            Debug.LogWarning("[CieloBeatManager] No se encontró el objeto 'obstacles'.");
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (source == null || !source.isPlaying || secondsPerBeat <= 0f) return;

        int beat = Mathf.FloorToInt(source.time / secondsPerBeat);
        if (beat != lastBeat)
        {
            lastBeat = beat;
            OnBeat?.Invoke();
        }
    }
}

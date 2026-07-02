using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Se auto-inyecta en level_inferno al iniciar. Lee el AudioSource del objeto "musicManager"
/// y emite OnBeat a 150 BPM. También adjunta InfernoBeatScalePulse al objeto "obstacles".
/// </summary>
public class InfernoBeatManager : MonoBehaviour
{
    public static InfernoBeatManager Instance { get; private set; }
    public event Action OnBeat;

    const string SceneName = "level_inferno"; // Asegúrate de que el nombre de tu escena sea exactamente este
    const float BPM = 150f; // BPM de la canción del infierno

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
            Debug.LogWarning("[InfernoBeatManager] No se encontró el objeto 'musicManager'.");
            return;
        }

        var mgr = musicObj.AddComponent<InfernoBeatManager>();
        mgr.source = musicObj.GetComponent<AudioSource>();
        mgr.secondsPerBeat = 60f / BPM;
        Instance = mgr;

        var obstaclesObj = GameObject.Find("obstacles");
        if (obstaclesObj != null)
            obstaclesObj.AddComponent<InfernoBeatScalePulse>();
        else
            Debug.LogWarning("[InfernoBeatManager] No se encontró el objeto 'obstacles'.");
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
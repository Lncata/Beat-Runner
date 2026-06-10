using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensambla SOLO la capa de UI (estado, puntaje, música opcional, HUD, resultados y EventSystem) en
/// tiempo de ejecución. No crea ni lee la meta, los obstáculos, el fondo ni la cámara: esos los
/// aporta tu nivel y se conecta por el contrato de integración (§3).
/// </summary>
public static class GameBootstrapper
{
    const string TargetScene = "level_Earth_01";  // ← nombre EXACTO de tu escena

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

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySetup(scene);
    }

    static void TrySetup(Scene scene)
    {
        if (scene.name != TargetScene)
        {
            return;
        }
        if (GameManager.Instance != null)
        {
            return; // Ya inicializado para esta escena.
        }

        EnsureEventSystem();

        var root = new GameObject("=== UI Systems (runtime) ===");
        var score = root.AddComponent<ScoreManager>();
        var music = root.AddComponent<MusicManager>();
        var hud = root.AddComponent<HUDController>();
        var results = root.AddComponent<ResultsScreen>();
        var game = root.AddComponent<GameManager>();

        score.Initialize(scene.name);
        music.Initialize();
        results.Initialize(game, score);
        game.Initialize(score, music, hud, results);
        hud.Initialize(game, score, music);

        game.StartRun();
        Debug.Log("[UI Bootstrap] Capa de UI lista.");
    }

    static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }
        // La escena puede no traer EventSystem; los botones de Resultados lo necesitan.
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}

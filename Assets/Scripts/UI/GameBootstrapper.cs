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
    static bool IsValidScene(string sceneName)
    {
        return sceneName == "level_Earth_01" || sceneName == "level_inferno" || sceneName == "level_cielo";
    }

    static bool subscribed;

    static void TrySetup(Scene scene)
    {
        if (!IsValidScene(scene.name))
        {
            return; // No es un nivel jugable.
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

        // Configurar canción dependiendo de la escena
        if (scene.name == "level_inferno")
        {
            music.resourcePath = "Music/CFC MADROCK"; // Nombre del archivo SIN el .mp3
            music.bpm = 150f; // Pon aquí el BPM real de la canción de rock
        }
        else if (scene.name == "level_Earth_01")
        {
            music.resourcePath = "Music/level_tierra";
        }

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

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Won,
    Dead
}

/// <summary>
/// Estado del juego para la UI: vidas, victoria/derrota, pausa y reinicio. NO conoce el mapa, los
/// obstáculos ni el jugador: tu nivel le avisa mediante RegisterObstacleHit() / Win() / ReportProgress().
/// Lo crea y configura GameBootstrapper en tiempo de ejecución.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Vidas")]
    public int startingLives = 3;

    const string MenuSceneName = "MainMenu";

    public GameState State { get; private set; } = GameState.Playing;
    public int CurrentLives { get; private set; }
    public int HitsTaken { get; private set; }

    public event Action<int> OnLivesChanged;
    public event Action<GameState> OnStateChanged;

    ScoreManager score;
    MusicManager music;
    HUDController hud;
    ResultsScreen results;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Initialize(ScoreManager score, MusicManager music, HUDController hud, ResultsScreen results)
    {
        this.score = score;
        this.music = music;
        this.hud = hud;
        this.results = results;
    }

    public void StartRun()
    {
        Time.timeScale = 1f;
        State = GameState.Playing;
        CurrentLives = Mathf.Max(1, startingLives);
        HitsTaken = 0;

        score?.BeginRun();
        music?.Play();

        OnLivesChanged?.Invoke(CurrentLives);
        OnStateChanged?.Invoke(State);
    }

    void Update()
    {
        if (State == GameState.Playing)
        {
            score?.Tick(Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (State != GameState.Playing && State != GameState.Paused && Input.GetKeyDown(KeyCode.R))
        {
            Retry();
        }
    }

    // === API que llama TU nivel (contrato de integración, §3) ===

    /// <summary>Avance del nivel 0..1, calculado por tu nivel. Llena la barra del HUD.</summary>
    public void ReportProgress(float progress01)
    {
        if (State == GameState.Playing)
        {
            score?.SetProgress(progress01);
        }
    }

    /// <summary>Choque con un obstáculo. Devuelve true si el jugador sigue vivo.</summary>
    public bool RegisterObstacleHit()
    {
        if (State != GameState.Playing)
        {
            return false;
        }

        HitsTaken++;
        CurrentLives--;
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives <= 0)
        {
            Lose();
            return false;
        }
        return true;
    }

    public void Win()
    {
        if (State != GameState.Playing)
        {
            return;
        }
        State = GameState.Won;
        OnStateChanged?.Invoke(State);

        music?.Stop();
        score?.FinalizeRun(true, HitsTaken);
        results?.Show(true);
    }

    public void Lose()
    {
        if (State != GameState.Playing)
        {
            return;
        }
        State = GameState.Dead;
        OnStateChanged?.Invoke(State);

        music?.Stop();
        score?.FinalizeRun(false, HitsTaken);
        results?.Show(false);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuSceneName);
    }

    void TogglePause()
    {
        if (State == GameState.Playing)
        {
            State = GameState.Paused;
            Time.timeScale = 0f;
            music?.Pause();
            OnStateChanged?.Invoke(State);
        }
        else if (State == GameState.Paused)
        {
            State = GameState.Playing;
            Time.timeScale = 1f;
            music?.Resume();
            OnStateChanged?.Invoke(State);
        }
    }
}

using UnityEngine;

/// <summary>
/// Modelo de puntaje del lado de la UI: progreso del nivel, puntaje, calificación S/A/B/C y mejor
/// puntaje (PlayerPrefs). NO lee el mapa: el progreso lo reporta tu nivel con SetProgress(0..1).
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Puntuación")]
    [Tooltip("Puntos al alcanzar el 100% del recorrido.")]
    public float fullRunScore = 10000f;
    [Tooltip("Puntos por segundo sobrevivido.")]
    public float timePoints = 50f;
    [Tooltip("Bono al completar el nivel.")]
    public int completionBonus = 5000;

    const string NoGrade = "—";

    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public string Grade { get; private set; } = NoGrade;
    public float Progress { get; private set; }
    public bool Completed { get; private set; }

    float runTime;
    string bestScoreKey;

    public void Initialize(string levelKey)
    {
        bestScoreKey = "BestScore_" + levelKey;
        BestScore = PlayerPrefs.GetInt(bestScoreKey, 0);
    }

    public void BeginRun()
    {
        Score = 0;
        runTime = 0f;
        Progress = 0f;
        Completed = false;
        Grade = NoGrade;
    }

    /// <summary>Lo llama tu nivel cada frame con el avance 0..1 (distancia / longitud del nivel).</summary>
    public void SetProgress(float progress01)
    {
        Progress = Mathf.Clamp01(progress01);
    }

    /// <summary>Lo llama GameManager con el delta de tiempo mientras se juega.</summary>
    public void Tick(float deltaTime)
    {
        runTime += deltaTime;
        Score = Mathf.RoundToInt(Progress * fullRunScore + runTime * timePoints);
    }

    public void FinalizeRun(bool completed, int hitsTaken)
    {
        Completed = completed;
        if (completed)
        {
            Score += completionBonus;
        }
        Grade = ComputeGrade(completed, hitsTaken);

        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(bestScoreKey, BestScore);
            PlayerPrefs.Save();
        }
    }

    static string ComputeGrade(bool completed, int hitsTaken)
    {
        if (!completed)
        {
            return NoGrade;
        }
        if (hitsTaken <= 0)
        {
            return "S";
        }
        if (hitsTaken == 1)
        {
            return "A";
        }
        if (hitsTaken == 2)
        {
            return "B";
        }
        return "C";
    }
}

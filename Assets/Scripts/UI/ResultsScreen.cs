using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pantalla de resultados por código: título, calificación grande, puntaje, mejor puntaje
/// y botones Reintentar / Menú. Se muestra al ganar o perder.
/// </summary>
public class ResultsScreen : MonoBehaviour
{
    static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
    static readonly Color Cyan = new Color(0.2f, 0.9f, 1f);
    static readonly Color Red = new Color(1f, 0.25f, 0.35f);
    static readonly Color Magenta = new Color(1f, 0.2f, 0.8f);
    static readonly Color Dim = new Color(0.7f, 0.7f, 0.8f);

    GameManager game;
    ScoreManager score;
    bool shown;

    public void Initialize(GameManager game, ScoreManager score)
    {
        this.game = game;
        this.score = score;
    }

    public void Show(bool completed)
    {
        if (shown)
        {
            return;
        }
        shown = true;
        BuildUi(completed);
    }

    void BuildUi(bool completed)
    {
        Canvas canvas = UiFactory.CreateCanvas("Results Canvas", 200);
        canvas.transform.SetParent(transform, false);
        Transform root = canvas.transform;

        Image background = UiFactory.CreatePanel(root, new Color(0.02f, 0.01f, 0.05f, 0.85f));
        UiFactory.Stretch(background);

        string title = completed ? "¡NIVEL COMPLETADO!" : "GAME OVER";
        Text titleText = UiFactory.CreateText(root, title, 70, completed ? Cyan : Red, TextAnchor.MiddleCenter);
        UiFactory.Place(titleText, Center, Center, Center, new Vector2(0, 260), new Vector2(1200, 120));

        string gradeValue = score != null ? score.Grade : "—";
        Text gradeText = UiFactory.CreateText(root, gradeValue, 200, GradeColor(gradeValue), TextAnchor.MiddleCenter);
        UiFactory.Place(gradeText, Center, Center, Center, new Vector2(0, 60), new Vector2(400, 280));

        int finalScore = score != null ? score.Score : 0;
        int bestScore = score != null ? score.BestScore : 0;

        Text scoreText = UiFactory.CreateText(root, $"PUNTAJE: {finalScore}", 44, Color.white, TextAnchor.MiddleCenter);
        UiFactory.Place(scoreText, Center, Center, Center, new Vector2(0, -110), new Vector2(900, 60));

        Text bestText = UiFactory.CreateText(root, $"MEJOR: {bestScore}", 30, Dim, TextAnchor.MiddleCenter);
        UiFactory.Place(bestText, Center, Center, Center, new Vector2(0, -160), new Vector2(900, 50));

        Button retry = UiFactory.CreateButton(root, "REINTENTAR", Magenta, Color.white,
            () => game?.Retry());
        UiFactory.Place(retry.image, Center, Center, Center, new Vector2(-170, -280), new Vector2(300, 90));

        Button menu = UiFactory.CreateButton(root, "MENÚ", Cyan, Color.black,
            () => game?.ReturnToMenu());
        UiFactory.Place(menu.image, Center, Center, Center, new Vector2(170, -280), new Vector2(300, 90));
    }

    static Color GradeColor(string grade)
    {
        switch (grade)
        {
            case "S": return new Color(1f, 0.85f, 0.2f);   // dorado
            case "A": return new Color(0.4f, 1f, 0.5f);    // verde
            case "B": return Cyan;
            case "C": return new Color(1f, 0.55f, 0.2f);   // naranja
            default: return Dim;
        }
    }
}

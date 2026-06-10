using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD por código: puntaje, mejor puntaje, vidas, barra de progreso, punto que late con el beat y
/// overlay de pausa. Cero cableado de assets — usa UiFactory y la fuente builtin.
/// </summary>
public class HUDController : MonoBehaviour
{
    static readonly Color Neon = new Color(1f, 0.2f, 0.8f);        // fucsia
    static readonly Color Cyan = new Color(0.2f, 0.9f, 1f);
    static readonly Color Dim = new Color(0.7f, 0.7f, 0.8f);
    static readonly Color PanelBg = new Color(0.04f, 0.02f, 0.08f, 0.55f);

    const float BeatPulseScale = 1.8f;
    const float BeatDecaySpeed = 8f;

    GameManager game;
    ScoreManager score;
    MusicManager music;

    Text scoreText;
    Text bestText;
    Text livesText;
    Image progressFill;
    Image beatDot;
    GameObject pauseOverlay;

    public void Initialize(GameManager game, ScoreManager score, MusicManager music)
    {
        this.game = game;
        this.score = score;
        this.music = music;

        BuildUi();

        if (game != null)
        {
            game.OnLivesChanged += UpdateLives;
            game.OnStateChanged += HandleStateChanged;
            UpdateLives(game.CurrentLives);
        }
        if (music != null)
        {
            music.OnBeat += PulseBeat;
        }
    }

    void OnDestroy()
    {
        if (game != null)
        {
            game.OnLivesChanged -= UpdateLives;
            game.OnStateChanged -= HandleStateChanged;
        }
        if (music != null)
        {
            music.OnBeat -= PulseBeat;
        }
    }

    void BuildUi()
    {
        Canvas canvas = UiFactory.CreateCanvas("HUD Canvas", 100);
        canvas.transform.SetParent(transform, false);
        Transform root = canvas.transform;

        scoreText = UiFactory.CreateText(root, "PUNTAJE: 0", 40, Cyan, TextAnchor.UpperLeft);
        UiFactory.Place(scoreText, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(40, -30), new Vector2(600, 60));

        bestText = UiFactory.CreateText(root, "MEJOR: 0", 26, Dim, TextAnchor.UpperLeft);
        UiFactory.Place(bestText, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(40, -78), new Vector2(600, 40));

        livesText = UiFactory.CreateText(root, "VIDAS: 3", 40, Neon, TextAnchor.UpperRight);
        UiFactory.Place(livesText, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-40, -30), new Vector2(400, 60));

        // Barra de progreso (abajo centro).
        Image barBackground = UiFactory.CreatePanel(root, PanelBg);
        UiFactory.Place(barBackground, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(0, 44), new Vector2(900, 26));

        progressFill = UiFactory.CreatePanel(barBackground.transform, Cyan);
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;
        progressFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        progressFill.fillAmount = 0f;
        RectTransform fillRect = progressFill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(3, 3);
        fillRect.offsetMax = new Vector2(-3, -3);

        UiFactory.CreateText(barBackground.transform, "INICIO", 18, Dim, TextAnchor.MiddleLeft);
        Text metaLabel = UiFactory.CreateText(barBackground.transform, "META", 18, Dim, TextAnchor.MiddleRight);
        UiFactory.Place(metaLabel, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f),
            new Vector2(-6, 22), new Vector2(120, 30));

        // Punto que late con el beat (arriba centro).
        beatDot = UiFactory.CreatePanel(root, Neon);
        UiFactory.Place(beatDot, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1),
            new Vector2(0, -40), new Vector2(22, 22));

        // Overlay de pausa (oculto al inicio).
        pauseOverlay = BuildPauseOverlay(root);
        pauseOverlay.SetActive(false);
    }

    GameObject BuildPauseOverlay(Transform root)
    {
        Image bg = UiFactory.CreatePanel(root, new Color(0f, 0f, 0f, 0.6f));
        UiFactory.Stretch(bg);

        Text label = UiFactory.CreateText(bg.transform, "PAUSA", 90, Cyan, TextAnchor.MiddleCenter);
        UiFactory.Place(label, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 40), new Vector2(800, 140));

        Text hint = UiFactory.CreateText(bg.transform, "ESC para continuar", 32, Dim, TextAnchor.MiddleCenter);
        UiFactory.Place(hint, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -70), new Vector2(800, 60));

        return bg.gameObject;
    }

    void Update()
    {
        if (score != null)
        {
            scoreText.text = $"PUNTAJE: {score.Score}";
            bestText.text = $"MEJOR: {Mathf.Max(score.BestScore, score.Score)}";
            if (progressFill != null)
            {
                progressFill.fillAmount = score.Progress;
            }
        }

        if (beatDot != null)
        {
            beatDot.transform.localScale = Vector3.Lerp(
                beatDot.transform.localScale, Vector3.one, BeatDecaySpeed * Time.unscaledDeltaTime);
        }
    }

    void PulseBeat(int beat)
    {
        if (beatDot != null)
        {
            beatDot.transform.localScale = Vector3.one * BeatPulseScale;
        }
    }

    void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "VIDAS: " + Mathf.Max(0, lives);
        }
    }

    void HandleStateChanged(GameState state)
    {
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(state == GameState.Paused);
        }
    }
}

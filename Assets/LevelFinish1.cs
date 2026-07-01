using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelFinish1 : MonoBehaviour
{
    public int desbloquearNivel = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Desbloquear siguiente nivel
        int nivelActual = PlayerPrefs.GetInt("NivelDesbloqueado", 1);
        if (desbloquearNivel > nivelActual)
        {
            PlayerPrefs.SetInt("NivelDesbloqueado", desbloquearNivel);
            PlayerPrefs.Save();
        }

        Time.timeScale = 0f;
        ShowResultsScreen();
    }

    void ShowResultsScreen()
    {
        float multiplier = CieloBuffManager.Instance != null
            ? CieloBuffManager.Instance.ScoreMultiplier
            : 1f;

        string rank      = CieloScoreManager.GetRank(multiplier);
        Color  rankColor = CieloScoreManager.GetRankColor(rank);
        int    deaths    = CieloScoreManager.Deaths;

        // Canvas de resultados
        var canvasGo = new GameObject("ResultsCanvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Fondo oscuro
        var overlay = new GameObject("Overlay");
        overlay.transform.SetParent(canvasGo.transform, false);
        var overlayImg = overlay.AddComponent<Image>();
        overlayImg.color         = new Color(0f, 0f, 0f, 0.85f);
        overlayImg.raycastTarget = false;
        var ort = overlay.GetComponent<RectTransform>();
        ort.anchorMin = Vector2.zero; ort.anchorMax = Vector2.one;
        ort.offsetMin = ort.offsetMax = Vector2.zero;

        // Panel central
        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.07f, 0.07f, 0.18f);
        SetCenter(panel.GetComponent<RectTransform>(), new Vector2(360, 420), Vector2.zero);

        // Título
        MakeTMP("Titulo", panel.transform, "NIVEL COMPLETADO", 28,
                Color.white, new Vector2(0, 155), new Vector2(330, 44));

        // Muertes
        string deathText = deaths == 0 ? "Sin muertes  ✓" : $"Muertes: {deaths}";
        MakeTMP("Deaths", panel.transform, deathText, 18,
                deaths == 0 ? Color.green : new Color(1f, 0.6f, 0.6f),
                new Vector2(0, 90), new Vector2(300, 32));

        // Buff activo
        if (multiplier >= 2f)
        {
            int effectiveDeaths = deaths / 2;
            MakeTMP("Buff", panel.transform,
                    $"★ x2 SCORE  →  muertes efectivas: {effectiveDeaths}",
                    15, new Color(1f, 0.85f, 0.1f),
                    new Vector2(0, 55), new Vector2(330, 26));
        }

        // Rango (letra grande)
        var rankLbl = MakeTMP("RankLabel", panel.transform, "RANGO", 16,
                              new Color(0.7f, 0.7f, 0.7f),
                              new Vector2(0, 5), new Vector2(200, 26));

        var rankTxt = MakeTMP("Rank", panel.transform, rank, 90,
                              rankColor, new Vector2(0, -65), new Vector2(140, 110));
        rankTxt.fontStyle = FontStyles.Bold;

        // Botón CONTINUAR
        var btnGo = new GameObject("BtnContinuar");
        btnGo.transform.SetParent(panel.transform, false);
        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.1f, 0.55f, 0.2f);
        SetCenter(btnGo.GetComponent<RectTransform>(), new Vector2(180, 48), new Vector2(0, -168));
        var btn = btnGo.AddComponent<Button>();
        btn.onClick.AddListener(GoToMenu);
        MakeTMP("Lbl", btnGo.transform, "CONTINUAR", 20,
                Color.white, Vector2.zero, new Vector2(180, 48)).fontStyle = FontStyles.Bold;
    }

    void GoToMenu()
    {
        CieloScoreManager.Reset();
        Time.timeScale = 1f;
        MainMenuController.abrirSeleccionNiveles = true;
        SceneManager.LoadScene("MainMenu");
    }

    // -------------------------------------------------------------------------

    static void SetCenter(RectTransform rt, Vector2 size, Vector2 pos)
    {
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.sizeDelta        = size;
        rt.anchoredPosition = pos;
    }

    static TextMeshProUGUI MakeTMP(string name, Transform parent, string text,
                                   int size, Color color, Vector2 pos, Vector2 sizeDelta)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        SetCenter(go.GetComponent<RectTransform>(), sizeDelta, pos);
        return tmp;
    }
}

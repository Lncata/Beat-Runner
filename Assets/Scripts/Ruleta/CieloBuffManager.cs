using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Guarda el buff activo de la ruleta durante una partida del nivel cielo.
/// Se destruye al recargar la escena (muerte), así cada run empieza con ruleta fresca.
/// </summary>
public class CieloBuffManager : MonoBehaviour
{
    public static CieloBuffManager Instance { get; private set; }

    public RuletaReward? ActiveReward  { get; private set; }
    public int           ExtraLives    { get; private set; }
    public float         ScoreMultiplier { get; private set; } = 1f;
    public bool          IsCatForm     { get; private set; }

    // HUD creado en código (un Canvas pequeño con indicadores de buff)
    GameObject hudCanvas;
    TMP_Text   extraLifeLabel;   // "♥ +1"
    TMP_Text   scoreLabel;       // "★ x2"
    Image      flashOverlay;     // parpadeo rojo al absorber un golpe

    // -------------------------------------------------------------------------

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        BuildHUD();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (hudCanvas != null) Destroy(hudCanvas);
    }

    // -------------------------------------------------------------------------
    // Aplicar premio de la ruleta
    // -------------------------------------------------------------------------

    public void ApplyReward(RuletaReward reward)
    {
        ActiveReward = reward;
        switch (reward)
        {
            case RuletaReward.VidaExtra:
                ExtraLives = 1;
                if (extraLifeLabel != null) extraLifeLabel.gameObject.SetActive(true);
                break;

            case RuletaReward.MultiplicadorScore:
                ScoreMultiplier = 2f;
                if (scoreLabel != null) scoreLabel.gameObject.SetActive(true);
                break;

            case RuletaReward.TransformacionGato:
                IsCatForm = true;
                ApplyCatTransformToPlayer();
                break;
        }
    }

    /// <summary>
    /// Llamado desde PlayerMovementInferno al chocar con obstáculo.
    /// Devuelve true si el golpe fue absorbido (jugador sobrevive).
    /// </summary>
    public bool ConsumeExtraLife()
    {
        if (ExtraLives <= 0) return false;
        ExtraLives--;

        // Apagar indicador en HUD y mostrar parpadeo rojo
        if (extraLifeLabel != null) extraLifeLabel.gameObject.SetActive(false);
        StartCoroutine(FlashRed());
        return true;
    }

    // -------------------------------------------------------------------------
    // Transformación gato
    // -------------------------------------------------------------------------

    void ApplyCatTransformToPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var switcher = player.GetComponent<PlayerFormSwitcher>();
        if (switcher != null)
        {
            switcher.TransformToCat();
        }
        else
        {
            // Fallback si no hay PlayerFormSwitcher: achica el player directamente
            player.transform.localScale *= 0.55f;
            var mov = player.GetComponent<PlayerMovementInferno>();
            if (mov != null)
            {
                mov.jumpForce      = 10f;
                mov.fallMultiplier = 5.5f;
                mov.riseMultiplier = 2.2f;
            }
        }
    }

    // -------------------------------------------------------------------------
    // HUD de buffs (canvas creado en código, pequeño y en esquina)
    // -------------------------------------------------------------------------

    void BuildHUD()
    {
        hudCanvas = new GameObject("BuffHUD");
        var canvas = hudCanvas.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        hudCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        hudCanvas.AddComponent<GraphicRaycaster>();

        // Flash rojo (cubre toda la pantalla, transparente por defecto)
        var flashGo = new GameObject("Flash");
        flashGo.transform.SetParent(hudCanvas.transform, false);
        flashOverlay = flashGo.AddComponent<Image>();
        flashOverlay.color = new Color(1f, 0f, 0f, 0f);
        flashOverlay.raycastTarget = false;
        var frt = flashGo.GetComponent<RectTransform>();
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = frt.offsetMax = Vector2.zero;

        // Indicador vida extra (esquina superior izquierda)
        extraLifeLabel = MakeCornerLabel("ExtraLife", "♥  VIDA EXTRA", new Color(1f, 0.3f, 0.4f), new Vector2(10, -10));
        extraLifeLabel.gameObject.SetActive(false);

        // Indicador score x2 (debajo del de vida extra)
        scoreLabel = MakeCornerLabel("ScoreX2", "★  SCORE  x2", new Color(1f, 0.85f, 0.1f), new Vector2(10, -50));
        scoreLabel.gameObject.SetActive(false);
    }

    TMP_Text MakeCornerLabel(string name, string text, Color color, Vector2 offset)
    {
        var go = new GameObject(name);
        go.transform.SetParent(hudCanvas.transform, false);
        var txt = go.AddComponent<TextMeshProUGUI>();
        txt.text      = text;
        txt.fontSize  = 18;
        txt.color     = color;
        txt.fontStyle = FontStyles.Bold;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(0f, 1f);
        rt.pivot            = new Vector2(0f, 1f);
        rt.sizeDelta        = new Vector2(200, 36);
        rt.anchoredPosition = offset;
        return txt;
    }

    IEnumerator FlashRed()
    {
        if (flashOverlay == null) yield break;

        // Aparece rojo y desaparece en 0.4 segundos
        flashOverlay.color = new Color(1f, 0f, 0f, 0.45f);
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            flashOverlay.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.45f, 0f, t / 0.4f));
            yield return null;
        }
        flashOverlay.color = new Color(1f, 0f, 0f, 0f);
    }
}

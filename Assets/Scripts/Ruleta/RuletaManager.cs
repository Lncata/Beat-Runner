using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Lógica de la ruleta. Ponélo en un GameObject de la escena level_cielo
/// y arrastrá las referencias en el Inspector.
///
/// La rueda se detiene así (sector que queda bajo el puntero superior):
///   0°   → Vida Extra
///   120° → Score x2
///   240° → Modo Gato
/// Diseñá tu imagen de rueda con esos 3 sectores en ese orden horario.
/// </summary>
public class RuletaManager : MonoBehaviour
{
    [Header("Panel — el objeto raíz de toda la UI de ruleta")]
    [SerializeField] GameObject ruletaPanel;

    [Header("Transform de la imagen de rueda que gira")]
    [SerializeField] Transform wheelTransform;

    [Header("Botones")]
    [SerializeField] Button spinButton;
    [SerializeField] Button continueButton;

    [Header("Texto que muestra el resultado (opcional si usas imágenes)")]
    [SerializeField] TMP_Text resultText;

    [Header("Imágenes de resultado — arrastrá una por premio (opcional)")]
    [SerializeField] UnityEngine.UI.Image resultImage;
    [SerializeField] Sprite spriteVidaExtra;
    [SerializeField] Sprite spriteScoreX2;
    [SerializeField] Sprite spriteModoGato;

    [Header("AudioSource del musicManager (para pausar/reanudar)")]
    [SerializeField] AudioSource musicSource;

    // Ángulo de parada por premio (sector que queda en la parte de arriba del puntero)
    static readonly float[] StopAngles = { 0f, 120f, 240f };

    void Awake()
    {
        // Garantiza que CieloBuffManager exista en la escena
        if (CieloBuffManager.Instance == null)
            gameObject.AddComponent<CieloBuffManager>();
    }

    void Start()
    {
        if (musicSource == null)
        {
            var mm = GameObject.Find("musicManager");
            if (mm != null) musicSource = mm.GetComponent<AudioSource>();
            if (musicSource == null)
                musicSource = FindFirstObjectByType<AudioSource>();
        }

        if (musicSource != null) musicSource.Pause();
        Time.timeScale = 0f;

        ruletaPanel.SetActive(true);
        if (resultText != null) resultText.text = "";
        if (resultImage != null) resultImage.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        spinButton.onClick.AddListener(OnSpinClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    void OnSpinClicked()
    {
        spinButton.interactable = false;
        int rewardIndex = Random.Range(0, 3);
        StartCoroutine(SpinCoroutine((RuletaReward)rewardIndex));
    }

    IEnumerator SpinCoroutine(RuletaReward reward)
    {
        float startAngle = wheelTransform.localEulerAngles.z;
        float fullSpins  = Random.Range(5, 8) * 360f;
        float targetAngle = startAngle + fullSpins + StopAngles[(int)reward];
        float duration   = 3f;
        float elapsed    = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;          // unscaled porque timeScale = 0
            float t    = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 3f);  // ease-out cúbico
            wheelTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startAngle, targetAngle, ease));
            yield return null;
        }

        wheelTransform.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
        ShowResult(reward);
    }

    void ShowResult(RuletaReward reward)
    {
        string[] mensajes =
        {
            "¡VIDA EXTRA! Sobrevives un golpe adicional.",
            "¡SCORE x2! Tus puntos valen el doble.",
            "¡MODO GATO! Te transformas en gato."
        };

        Color[] colores =
        {
            new Color(1f, 0.3f, 0.4f),   // rosa  → Vida Extra
            new Color(1f, 0.85f, 0.1f),  // dorado → Score x2
            new Color(0.6f, 0.3f, 1f),   // morado → Gato
        };

        // Texto con color
        if (resultText != null)
        {
            resultText.text  = mensajes[(int)reward];
            resultText.color = colores[(int)reward];
        }

        // Imagen según premio (si está asignada)
        if (resultImage != null)
        {
            Sprite[] sprites = { spriteVidaExtra, spriteScoreX2, spriteModoGato };
            Sprite   sp      = sprites[(int)reward];
            if (sp != null)
            {
                resultImage.sprite  = sp;
                resultImage.gameObject.SetActive(true);
            }
        }

        continueButton.gameObject.SetActive(true);

        CieloBuffManager.Instance?.ApplyReward(reward);
    }

    void OnContinueClicked()
    {
        Time.timeScale = 1f;
        if (musicSource != null) musicSource.Play();
        ruletaPanel.SetActive(false);
    }
}

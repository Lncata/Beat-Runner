using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelFinish1 : MonoBehaviour
{
    public int desbloquearNivel = 2;

    [Header("Panel de resultados — diseñalo vos en el Editor")]
    [SerializeField] GameObject resultsPanel;

    [Header("Textos — arrastrá cada TMP_Text del panel")]
    [SerializeField] TMP_Text rankText;       // la letra grande S / A / B / C / D
    [SerializeField] TMP_Text deathsText;     // "Muertes: 2" o "Sin muertes"
    [SerializeField] TMP_Text buffText;       // "★ x2 aplicado" (se oculta si no hay buff)

    [Header("Botón continuar")]
    [SerializeField] Button continueButton;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[LevelFinish1] tocado por: {collision.gameObject.name} tag: {collision.tag} | resultsPanel={resultsPanel}");
        if (!collision.CompareTag("Player")) return;

        int nivelActual = PlayerPrefs.GetInt("NivelDesbloqueado", 1);
        if (desbloquearNivel > nivelActual)
        {
            PlayerPrefs.SetInt("NivelDesbloqueado", desbloquearNivel);
            PlayerPrefs.Save();
        }

        // Si no hay panel asignado, va directo al menú
        if (resultsPanel == null)
        {
            GoToMenu();
            return;
        }

        Time.timeScale = 0f;
        ShowResults();
    }

    void ShowResults()
    {
        float  multiplier = CieloBuffManager.Instance != null ? CieloBuffManager.Instance.ScoreMultiplier : 1f;
        int    deaths     = CieloScoreManager.Deaths;
        string rank       = CieloScoreManager.GetRank(multiplier);
        Color  rankColor  = CieloScoreManager.GetRankColor(rank);

        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        if (rankText != null)
        {
            rankText.text  = rank;
            rankText.color = rankColor;
        }

        if (deathsText != null)
            deathsText.text = deaths == 0 ? "Sin muertes  ✓" : $"Muertes: {deaths}";

        if (buffText != null)
        {
            if (multiplier >= 2f)
            {
                buffText.gameObject.SetActive(true);
                buffText.text = $"★ x2 SCORE  →  muertes efectivas: {deaths / 2}";
            }
            else
            {
                buffText.gameObject.SetActive(false);
            }
        }

        if (continueButton != null)
            continueButton.onClick.AddListener(GoToMenu);
    }

    void GoToMenu()
    {
        CieloScoreManager.Reset();
        Time.timeScale = 1f;
        MainMenuController.abrirSeleccionNiveles = true;
        SceneManager.LoadScene("MainMenu");
    }
}

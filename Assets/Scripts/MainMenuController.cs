using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject levelPanel;
    public GameObject catgirl;
    public GameObject panelPrincipal;
    public GameObject panelNiveles;

    public Button botonCielo;
    public Button botonTierra;
    public Button botonInfierno;

    public GameObject candadoTierra;
    public GameObject candadoInfierno;

    public static bool abrirSeleccionNiveles = false;

void Start()
{
    int abrir = PlayerPrefs.GetInt("AbrirSeleccionNiveles", 0);

    Debug.Log("Abrir seleccion niveles = " + abrir);

    ActualizarNiveles();

    panelPrincipal.SetActive(abrir == 0);
    panelNiveles.SetActive(abrir == 1);

    PlayerPrefs.SetInt("AbrirSeleccionNiveles", 0);
    PlayerPrefs.Save();
}
public void AbrirSeleccionDeNiveles()
{
    panelPrincipal.SetActive(false);
    panelNiveles.SetActive(true);
}

    void ActualizarNiveles()
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 1);

        botonCielo.interactable = true;

        botonTierra.interactable = nivelDesbloqueado >= 2;
        botonInfierno.interactable = nivelDesbloqueado >= 3;

        candadoTierra.SetActive(nivelDesbloqueado < 2);
        candadoInfierno.SetActive(nivelDesbloqueado < 3);
    }

    public void Jugar()
    {
        panelPrincipal.SetActive(false);
        panelNiveles.SetActive(true);
    }

    public void ShowLevels()
    {
        levelPanel.SetActive(true);
        catgirl.SetActive(false);
    }

    public void HideLevels()
    {
        levelPanel.SetActive(false);
        catgirl.SetActive(true);
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }
    public void ResetProgress()
{
    PlayerPrefs.SetInt("NivelDesbloqueado", 1);
    PlayerPrefs.Save();

    ActualizarNiveles();

    Debug.Log("Progreso reiniciado");
}

    public void UnlockAllLevels()
    {
        // Establecemos el progreso en 3 para habilitar Cielo, Tierra e Infierno
        PlayerPrefs.SetInt("NivelDesbloqueado", 3);
        PlayerPrefs.Save();

        // Actualizamos la UI inmediatamente (quitar candados y habilitar botones)
        ActualizarNiveles();

        Debug.Log("¡Todos los niveles desbloqueados!");
    }

}
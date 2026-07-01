using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish1 : MonoBehaviour
{
    public int desbloquearNivel = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int nivelActual = PlayerPrefs.GetInt("NivelDesbloqueado", 1);

            if (desbloquearNivel > nivelActual)
            {
                PlayerPrefs.SetInt("NivelDesbloqueado", desbloquearNivel);
                PlayerPrefs.Save();
            }

            Time.timeScale = 1f;
            MainMenuController.abrirSeleccionNiveles = true;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
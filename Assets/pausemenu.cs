using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioSource musicSource;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        if (musicSource != null)
            musicSource.Pause();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

        if (musicSource != null)
            musicSource.UnPause();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (musicSource != null)
            musicSource.Stop();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;

        if (musicSource != null)
            musicSource.Stop();

        SceneManager.LoadScene("MainMenu");
    }
}

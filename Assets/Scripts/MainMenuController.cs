using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject levelPanel;

    public void ShowLevels()
    {
        levelPanel.SetActive(true);
    }

    public void HideLevels()
    {
        levelPanel.SetActive(false);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
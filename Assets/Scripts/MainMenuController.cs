using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject levelPanel;
    public GameObject catgirl;

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
        SceneManager.LoadScene(levelName);
    }
}
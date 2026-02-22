using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject HowToPlayScreen;
    public TMP_Text HowToPlayButtonText;
    public Button HowToPlayButton;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Pause()
    {
        PauseScreen.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        PauseScreen.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void HowToPlay()
    {
        HowToPlayScreen.gameObject.SetActive(true);
        HowToPlayButton.onClick.AddListener(HowToPlayMenuClose);
        HowToPlayButton.onClick.RemoveListener(HowToPlay);
        HowToPlayButtonText.text = "Back";
    }

    public void HowToPlayMenuClose()
    {
        HowToPlayScreen.gameObject.SetActive(false);
        HowToPlayButton.onClick.RemoveListener(HowToPlayMenuClose);
        HowToPlayButton.onClick.AddListener(HowToPlay);
        HowToPlayButtonText.text = "How to Play";
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}

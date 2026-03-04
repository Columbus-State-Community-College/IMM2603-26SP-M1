using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject HowToPlayScreen;
    public GameObject MainMenuPanel;
    public TMP_Text HowToPlayButtonText;
    public Button HowToPlayButton;
    private bool isPaused;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isPaused)
            UnPause();
        else
            Pause();
    }
    public void Pause()
    {
        isPaused = true;
        PauseScreen.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        isPaused = false;
        PauseScreen.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuClose()
    {
        MainMenuPanel.gameObject.SetActive(false);
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
        MainMenuPanel.gameObject.SetActive(true);
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

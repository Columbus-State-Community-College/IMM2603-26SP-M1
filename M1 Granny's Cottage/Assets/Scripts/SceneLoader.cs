using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject HowToPlayScreen;
    public GameObject CreditsPage;
    public GameObject submenuScreen;
    public GameObject MainMenuPanel;
    public TMP_Text HowToPlayButtonText;
    public TMP_Text HighScoreText;
    public Button HowToPlayButton;
    private string scoreFile = "HighScore.txt";
    private bool isPaused;
    private int isSubScreenEnabled = 0;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (HighScoreText != null && DataPersistenceManager.instance != null)
        {
            GameData data = DataPersistenceManager.instance.GameData;
            if (data != null)
            {
                HighScoreText.text = "High Score: " + data.highScore;
            }
            else
            {
                HighScoreText.text = "High Score: 0";
            }
        }
    }

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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnPause()
    {
        isPaused = false;
        PauseScreen.gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenuClose()
    {
        MainMenuPanel.gameObject.SetActive(false);
    }

    public void SubMenu()
    {

        if (isSubScreenEnabled == 0){
            submenuScreen.gameObject.SetActive(true);
            isSubScreenEnabled += 1;
        }
        
        else if (isSubScreenEnabled == 1){
            submenuScreen.gameObject.SetActive(false);
            isSubScreenEnabled = 0;
        }
        // Debug.Log("This is where the upgrade menu will be!");
    }

    public void HowToPlay()
    {
        HowToPlayScreen.gameObject.SetActive(true);
    }

    public void CreditsPageOpen()
    {
        CreditsPage.gameObject.SetActive(true);
    }

    public void CreditsPageClose()
    {
        CreditsPage.gameObject.SetActive(false);

        if (MainMenuPanel != null)
        {
            MainMenuPanel.gameObject.SetActive(true);
        }
        
        else 
        {
            Debug.Log("MainMenuPanel object is not assigned.");
        }
    }

    public void HowToPlayMenuClose()
    {
        HowToPlayScreen.gameObject.SetActive(false);

        if (MainMenuPanel != null)
        {
            MainMenuPanel.gameObject.SetActive(true);
        }
        
        else 
        {
            Debug.Log("MainMenuPanel object is not assigned.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}

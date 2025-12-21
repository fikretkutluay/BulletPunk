using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yüklemek için þart

public class MainMenuManager : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject mainMenu;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    // Oyna Butonu buna baðlanacak
    public void PlayGame()
    {
        // "GameScene" senin oyun sahnenin tam adý olmalý!
        SceneManager.LoadScene(1);
    }

    // Çýkýþ Butonu buna baðlanacak
    public void QuitGame()
    {
        Debug.Log("Oyundan Çýkýldý!"); // Editörde kapanmaz, log düþer
        Application.Quit();
    }

    public void CreditsPanel()
    {
        creditsPanel.SetActive(true);
    }

    public void MainMenu()
    {
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void SettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void TutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }


}
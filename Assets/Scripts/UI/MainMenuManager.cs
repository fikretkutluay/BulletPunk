using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yüklemek için þart

public class MainMenuManager : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject mainMenu;
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
        mainMenu.SetActive(true);
    }


}
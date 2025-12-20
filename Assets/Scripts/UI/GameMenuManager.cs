using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    // Oyun durdu mu kontrolü
    public static bool isGamePaused = false;

    // Geçici test tuþu (K) için deðiþken
    private bool isGameOver = false;

    void Update()
    {
        // GEÇÝCÝ: K tuþuna basýnca ÖLME TESTÝ
        // Arkadaþýn PlayerStats'ý bitirince burayý silersin.
        if (Input.GetKeyDown(KeyCode.K) && !isGameOver)
        {
            ShowGameOver();
        }

        // Eðer oyun bitmediyse ESC ile durdur/devam ettir
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Zamaný durdur
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Zamaný devam ettir
        isGamePaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Menüye dönerken zamaný düzelt!
        SceneManager.LoadScene(0);
    }

    // --- RESTART SORUNUNU ÇÖZEN KISIM ---
    public void RestartGame()
    {
        // KRÝTÝK: Sahne yüklenmeden önce zamaný normale döndürmelisin.
        // Yoksa oyun "Pause" modunda baþlar.
        Debug.Log("RESTART BUTONUNA BASILDI! SAHNE YÜKLENÝYOR...");

        Time.timeScale = 1f;

        SceneManager.LoadScene(1);
    }

    public void ShowGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Oyunu dondur
    }
}
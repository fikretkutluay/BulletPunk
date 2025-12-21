using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    // Singleton: Diðer scriptlerden (PlayerStats) ulaþmak için þart
    public static GameMenuManager Instance;

    [Header("Paneller")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    // Oyun durdu mu kontrolü
    public static bool isGamePaused = false;
    private bool isGameOver = false;

    private void Awake()
    {
        // Singleton Kurulumu
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // GEÇÝCÝ: K tuþu ile test (Ýþin bitince silersin)
        if (Input.GetKeyDown(KeyCode.K) && !isGameOver)
        {
            ShowGameOver();
        }

        // ESC Tuþu Kontrolü
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
        isGamePaused = false;
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        Debug.Log("RESTART BUTONUNA BASILDI! SAHNE YÜKLENÝYOR...");

        Time.timeScale = 1f; // Önce zamaný düzelt
        isGamePaused = false;

        // Mevcut sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);

        // Eðer Pause paneli açýksa onu kapat ki üst üste binmesin
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        Time.timeScale = 0f; // Oyunu dondur
    }
}
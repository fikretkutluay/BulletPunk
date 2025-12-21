using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // Slider eriþimi için gerekli
using UnityEngine.Audio; // Audio Mixer eriþimi için gerekli

public class GameMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    [Header("Ses Ayarlarý")]
    public AudioMixer mainMixer;       // Project panelindeki MainMixer'ý buraya atacaðýz
    public Slider musicSlider;         // Pause Menüdeki Müzik Slider'ý
    public Slider sfxSlider;           // Pause Menüdeki SFX Slider'ý

    // PlayerPrefs Anahtarlarý (Ana Menü ile AYNI olmalý)
    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";
    private const string MixerMusicParam = "MusicVol";
    private const string MixerSFXParam = "SFXVol";

    public static bool isGamePaused = false;
    private bool isGameOver = false;

    private void Start()
    {
        // 1. Sahne açýlýnca kayýtlý ses ayarlarýný yükle
        float savedMusicVol = PlayerPrefs.GetFloat(MusicKey, 1f);
        float savedSFXVol = PlayerPrefs.GetFloat(SFXKey, 1f);

        // 2. Slider'larýn konumunu ayarla (Eðer atanmýþlarsa)
        if (musicSlider) musicSlider.value = savedMusicVol;
        if (sfxSlider) sfxSlider.value = savedSFXVol;

        // 3. Sesi Mixer'a uygula
        // (Oyun sahnesi yeni yüklendiðinde sesin doðru seviyede baþlamasý için þart)
        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);
    }

    void Update()
    {
        // GEÇÝCÝ: K tuþuna basýnca ÖLME TESTÝ
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

    public void RestartGame()
    {
        Debug.Log("RESTART BUTONUNA BASILDI! SAHNE YÜKLENÝYOR...");
        Time.timeScale = 1f;
        // Build Settings'te aktif sahneyi yeniden yükler
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- SES KONTROLÜ (Slider Eventleri) ---
    // Bu fonksiyonlarý Slider'larýn "On Value Changed" kýsmýna baðlayacaksýn

    public void SetMusicVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(MusicKey, sliderValue);
        PlayerPrefs.Save();

        // Logaritmik hesaplama
        float mixerValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat(MixerMusicParam, mixerValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(SFXKey, sliderValue);
        PlayerPrefs.Save();

        float mixerValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat(MixerSFXParam, mixerValue);
    }
}
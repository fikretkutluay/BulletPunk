using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
public class MainMenuManager : MonoBehaviour
{
    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Vector2 hotSpot = Vector2.zero;
    [Header("Paneller")]
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject tutorialPanel;
    public GameObject settingsPanel;

    [Header("Ses Ayarlarý")]
    public AudioMixer mainMixer;       // YENÝ: AudioSource yerine Mixer alýyoruz
    public Slider musicSlider;
    public Slider sfxSlider;

    // PlayerPrefs ve Mixer Parametre Anahtarlarý
    private const string MusicKey = "MusicVolume"; // PlayerPrefs için
    private const string SFXKey = "SFXVolume";     // PlayerPrefs için

    // Mixer'da "Expose" ettiðimiz parametre isimleri (Adým 3'te verdiðin isimler)
    private const string MixerMusicParam = "MusicVol";
    private const string MixerSFXParam = "SFXVol";

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
        
        // 1. Kayýtlý veriyi çek (Yoksa 1 yani %100 ses gelir)
        float savedMusicVol = PlayerPrefs.GetFloat(MusicKey, 1f);
        float savedSFXVol = PlayerPrefs.GetFloat(SFXKey, 1f);

        // 2. Sliderlarý güncelle
        if (musicSlider) musicSlider.value = savedMusicVol;
        if (sfxSlider) sfxSlider.value = savedSFXVol;

        // 3. Sesi Mixer'a uygula (Mixer bazen Start'ta hemen güncellenmez, ufak bir gecikme gerekebilir ama genelde çalýþýr)
        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);

        OpenMainMenu();
    }

    // --- SES KONTROLÜ (Logaritmik Dönüþüm) ---

    public void SetMusicVolume(float sliderValue)
    {
        // PlayerPrefs'e normal (0-1) deðeri kaydet
        PlayerPrefs.SetFloat(MusicKey, sliderValue);
        PlayerPrefs.Save();

        // Mixer'a Logaritmik (-80dB ile 0dB) deðeri gönder
        // Slider 0.0001 altýna düþerse sesi tamamen kapat (-80dB)
        float mixerValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;

        mainMixer.SetFloat(MixerMusicParam, mixerValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(SFXKey, sliderValue);
        PlayerPrefs.Save();

        float mixerValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;

        mainMixer.SetFloat(MixerSFXParam, mixerValue);
    }

    // --- PANEL VE BUTON FONKSÝYONLARI (Ayný kaldý) ---

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Çýkýþ yapýldý.");
        Application.Quit();
    }

    private void ActivatePanel(GameObject activePanel)
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        settingsPanel.SetActive(false);
        activePanel.SetActive(true);
    }

    public void OpenMainMenu() => ActivatePanel(mainMenuPanel);
    public void OpenCredits() => ActivatePanel(creditsPanel);
    public void OpenTutorial() => ActivatePanel(tutorialPanel);
    public void OpenSettings() => ActivatePanel(settingsPanel);
}
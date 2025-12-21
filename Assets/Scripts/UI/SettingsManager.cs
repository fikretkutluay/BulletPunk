using UnityEngine;
using UnityEngine.Audio; // Mixer için þart
using UnityEngine.UI;    // Slider için þart

public class SettingsManager : MonoBehaviour
{
    [Header("Mixer Referansý")]
    public AudioMixer audioMixer; // Project'teki MainMixer'ý buraya atacaðýz

    [Header("Slider Referanslarý")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 1. Oyun açýlýnca kayýtlý ses ayarlarýný yükle (Yoksa 0.75 baþlasýn)
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        // 2. Sliderlarýn çubuðunu o konuma getir
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // 3. Sesi de o seviyeye ayarla (Sesleri hemen uygula)
        // Burayý elle çaðýrmýyoruz, sliderlar hareket edince aþaðýdakiler zaten çalýþacak
        // Ama açýlýþta garanti olsun diye:
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    // --- MÜZÝK AYARI ---
    public void SetMusicVolume(float volume)
    {
        // Mixer logaritmik çalýþýr (-80dB ile 0dB arasý)
        // Slider ise 0 ile 1 arasýdýr. Bu formül onu çevirir.
        // Eðer slider 0.0001 ise (en sol), ses -80 olur (tamamen kapanýr).
        float dbValue = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;

        audioMixer.SetFloat("MusicVolume", dbValue);

        // Ayarý kaydet
        PlayerPrefs.SetFloat("MusicVol", volume);
    }

    // --- SFX (EFEKT) AYARI ---
    public void SetSFXVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;

        audioMixer.SetFloat("SFXVolume", dbValue);

        PlayerPrefs.SetFloat("SFXVol", volume);
    }
}
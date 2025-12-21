using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Butonun aktifliðini kontrol etmek için

// Bu satýr sayesinde scripti attýðýn objeye otomatik AudioSource eklenir
[RequireComponent(typeof(AudioSource))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Görsel Ayarlar")]
    public float hoverScale = 1.1f;
    public float speed = 8f;

    [Header("Ses Ayarlarý")]
    public AudioClip hoverSound; // Üzerine gelme sesi (Bip)
    public AudioClip clickSound; // Týklama sesi (Çýt)
    [Range(0f, 1f)] public float soundVolume = 0.5f; // Ses þiddeti

    private Vector3 originalScale;
    private Vector3 targetScale;
    private AudioSource audioSource;
    private Button button;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        // Bileþenleri alýyoruz
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();

        // AudioSource ayarlarýný kodla yapalým (Uðraþma diye)
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0; // 2D ses olsun
    }

    void Update()
    {
        // Time.unscaledDeltaTime kullanman harika, pause menüsünde de çalýþýr
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    // --- MOUSE ÜZERÝNE GELÝNCE ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Eðer buton "Interactable" deðilse (sönükse) tepki verme
        if (button != null && !button.interactable) return;

        targetScale = originalScale * hoverScale;

        // Hover Sesi Çal
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound, soundVolume);
        }
    }

    // --- MOUSE ÜZERÝNDEN GÝDÝNCE ---
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    // --- TIKLAYINCA (YENÝ EKLENDÝ) ---
    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        // Click Sesi Çal
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, soundVolume);
        }
    }
}
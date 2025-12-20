using UnityEngine;
using UnityEngine.EventSystems; // Mouse olaylarýný yakalamak için þart

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ayarlar")]
    public float hoverScale = 1.1f; // Ne kadar büyüsün? (1.2 = %20 büyür)
    public float speed = 8f;       // Büyüme hýzý (Hollow Knight gibi yumuþak olmasý için)

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        // Baþlangýçtaki boyutunu kaydet (Genelde 1,1,1)
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Lerp fonksiyonu ile "yumuþak" geçiþ yap
        // Aniden büyümek yerine yavaþça hedef boyuta kayar
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    // Mouse üzerine gelince çalýþýr
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        // Ýstersen burada "Hover Sesi" de çaldýrabilirsin
        // AudioSource.PlayOneShot(hoverSound);
    }

    // Mouse üzerinden çekilince çalýþýr
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}
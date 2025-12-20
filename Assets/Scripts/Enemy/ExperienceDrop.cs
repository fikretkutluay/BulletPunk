using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    public int expAmount = 10;

    // Mýknatýs istemediðin için bu deðiþken þimdilik süs olarak kalabilir veya silebilirsin
    public float attractSpeed = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // "Player" tag kontrolü
        if (collision.CompareTag("Player"))
        {
            // --- BAÐLANTI KISMI ---

            // 1. Çarpan objenin üzerindeki PlayerStats scriptini bul
            PlayerStats stats = collision.GetComponent<PlayerStats>();

            // 2. Eðer script varsa XP'yi gönder
            if (stats != null)
            {
                stats.GainXP(expAmount);
                Debug.Log("Player picked up " + expAmount + " EXP!");

                // XP alýndý, taþý yok et
                Destroy(gameObject);
            }
        }
    }
}
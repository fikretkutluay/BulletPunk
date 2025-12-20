using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    // This is the value that will be added to the player's XP
    public float expAmount = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats stats = collision.GetComponent<PlayerStats>();

            if (stats != null)
            {
                // We use the amount set on this specific orb
                stats.GainXP(expAmount);
                Destroy(gameObject);
            }
        }
    }
}
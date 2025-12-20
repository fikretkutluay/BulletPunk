using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    public int expAmount = 10;
    public float attractSpeed = 5f; // For a "magnet" effect later if you want

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for the "PLAYER" tag as requested
        if (collision.CompareTag("Player"))
        {
            // Logic for adding EXP goes here 
            // (e.g., PlayerLevel.Instance.AddExp(expAmount))
            Debug.Log("Player picked up " + expAmount + " EXP!");

            Destroy(gameObject);
        }
    }
}
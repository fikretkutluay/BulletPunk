using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Slider slider;
    private Enemy enemyScript;

    public void Setup(Enemy enemy)
    {
        enemyScript = enemy;
        slider = GetComponentInChildren<Slider>();

        // Match slider max value to enemy max health
        slider.maxValue = enemy.health;
        slider.value = enemy.health;
    }

    void Update()
    {
        if (enemyScript != null)
        {
            // Update the slider value to match the enemy's current health
            slider.value = enemyScript.health;

            // Keep the health bar rotation static (doesn't flip if enemy flips)
            transform.rotation = Quaternion.identity;
        }
    }
}

using UnityEngine;

// This script allows you to test if the enemies are damaging you and locking your keys.
public class DummyPlayer : MonoBehaviour, IDamageable, IDebuffable
{
    public float moveSpeed = 5f;
    public float health = 100f;

    void Update()
    {
        Vector2 moveInput = Vector2.zero;

        // Check InputManager before allowing movement (WASD)
        if (Input.GetKey(KeyCode.W) && InputManager.Instance.IsControlActive(ControlType.MoveUp))
            moveInput.y += 1;
        if (Input.GetKey(KeyCode.S) && InputManager.Instance.IsControlActive(ControlType.MoveDown))
            moveInput.y -= 1;
        if (Input.GetKey(KeyCode.A) && InputManager.Instance.IsControlActive(ControlType.MoveLeft))
            moveInput.x -= 1;
        if (Input.GetKey(KeyCode.D) && InputManager.Instance.IsControlActive(ControlType.MoveRight))
            moveInput.x += 1;

        transform.Translate(moveInput.normalized * (moveSpeed * Time.deltaTime));

        // Test Skill Keys (Q, E, R, Space)
        if (Input.GetKeyDown(KeyCode.Q) && !InputManager.Instance.IsControlActive(ControlType.Skill1))
            Debug.Log("<color=red>Q is LOCKED!</color>");
        
        if (Input.GetKeyDown(KeyCode.E) && !InputManager.Instance.IsControlActive(ControlType.Skill2))
            Debug.Log("<color=red>E is LOCKED!</color>");

        if (Input.GetKeyDown(KeyCode.Space) && !InputManager.Instance.IsControlActive(ControlType.Dash))
            Debug.Log("<color=red>SPACE is LOCKED!</color>");
    }

    // Implementation of IDamageable
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Player took damage! Current Health: " + health);
    }

    // Implementation of IDebuffable
    public void LockControl(ControlType type, float duration)
    {
        // This directs the logic to your InputManager
        InputManager.Instance.ApplyLock(type, duration);
    }
}
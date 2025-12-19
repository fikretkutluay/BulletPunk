using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- SENÝN KULLANACAÐIN KISIM (PLAYER ÝÇÝN) ---
    // Player Scriptin Update'te: if (InputManager.Instance.IsControlActive(ControlType.Skill1))
    public bool IsControlActive(ControlType type)
    {
        // Þimdilik hep true döndür, sen mantýðýný sonra kurarsýn.
        // Arkadaþýn hata almadan çalýþabilsin diye var.
        return true;
    }

    // --- ARKADAÞININ KULLANACAÐI KISIM (ENEMY ÝÇÝN) ---
    // Düþman Scripti: InputManager.Instance.ApplyLock(ControlType.Movement, 2f);
    public void ApplyLock(ControlType type, float duration)
    {
        Debug.Log(type + " kontrolü " + duration + " saniyeliðine kilitlendi!");
        // Buranýn içini sen sonra dolduracaksýn.
        // Ama arkadaþýn þu an bu metodu çaðýrabilir, kod hatasý almaz.
    }
}
using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // Hangi tuþ, oyunun kaçýncý saniyesine kadar kilitli?
    // ControlType enum'ýný senin GameEnums.cs dosyasýndan otomatik okur.
    private Dictionary<ControlType, float> lockedControls = new Dictionary<ControlType, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne deðiþse bile InputManager yaþasýn
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- SORGULAMA KISMI (PLAYER & UI ÝÇÝN) ---
    // Player: if (InputManager.Instance.IsControlActive(ControlType.MoveUp))
    public bool IsControlActive(ControlType type)
    {
        // 1. Bu kontrol tipi kilitli listesinde var mý?
        if (lockedControls.ContainsKey(type))
        {
            // 2. Kilit süresi doldu mu?
            // Þimdiki zaman < Kilit Bitiþ Zamaný ise -> HALA KÝLÝTLÝ (false döndür)
            if (Time.time < lockedControls[type])
            {
                return false;
            }
            else
            {
                // Süre dolmuþ, listeden temizle (Artýk özgür)
                lockedControls.Remove(type);
                return true;
            }
        }

        // Listede yoksa kilitli deðildir, basýlabilir.
        return true;
    }

    // --- KÝLÝTLEME KISMI (ENEMY MERMÝLERÝ ÝÇÝN) ---
    // Enemy: InputManager.Instance.ApplyLock(ControlType.Skill1, 3f);
    public void ApplyLock(ControlType type, float duration)
    {
        // Kilit ne zaman bitecek? (Þu an + süre)
        float unlockTime = Time.time + duration;

        // Zaten kilitliyse ve yeni gelen kilit daha uzunsa süreyi uzat
        if (lockedControls.ContainsKey(type))
        {
            if (unlockTime > lockedControls[type])
            {
                lockedControls[type] = unlockTime;
            }
        }
        else
        {
            // Listede yoksa yeni kilit ekle
            lockedControls.Add(type, unlockTime);
        }

        Debug.Log($"GLITCH! {type} kontrolü {duration} saniyeliðine kilitlendi!");
    }

    // Opsiyonel: Tüm kilitleri kaldýr (Ölünce veya bölüm geçince)
    public void UnlockAllControls()
    {
        lockedControls.Clear();
    }
}
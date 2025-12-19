using UnityEngine;
public interface IDebuffable
{
    // Hangi kontrolü, kaç saniye kilitleyecek?
    void LockControl(ControlType type, float duration);
}

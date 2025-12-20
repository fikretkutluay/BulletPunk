using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        Instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void GenerateShake()
    {
        // This triggers the impulse signal defined in the Inspector
        impulseSource.GenerateImpulse();
    }
}
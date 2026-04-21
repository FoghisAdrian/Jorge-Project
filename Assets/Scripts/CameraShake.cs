using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin _perlin;
    private float _shakeTimer;
    private float _initialIntensity;

    void Awake()
    {
        Instance = this;
        _perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (_perlin != null)
        {
            _perlin.AmplitudeGain = 0f;
        }
        _shakeTimer = 0f;
    }

    public void Shake(float intensity, float time)
    {
        if (_perlin != null)
        {
            _perlin.AmplitudeGain = intensity;
            _shakeTimer = time;
            _initialIntensity = intensity;
        }
    }

    void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            _perlin.AmplitudeGain = Mathf.Lerp(0f, _initialIntensity, _shakeTimer / 0.1f);
        }
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    CinemachineVirtualCamera cmFreeCam;
    CinemachineBasicMultiChannelPerlin noise;
    float timer;
    void Start()
    {
        cmFreeCam = GetComponent<CinemachineVirtualCamera>();
        noise = cmFreeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Noise(float amplitudeGain, float frequencyGain)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
        StartCoroutine(ShakeScreen());
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }

    IEnumerator ShakeScreen()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                break;
            }
        }
        timer = 0f;
        yield return null;
    }
}

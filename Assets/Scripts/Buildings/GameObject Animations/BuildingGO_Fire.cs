using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGO_Fire : MonoBehaviour {

    bool isWorking = false;
    new public Light light;
    public ParticleSystem smoke;
    float warmupTime = 4f;
    float warmup = 0f;
    float maxIntensity;

    private void Awake()
    {
        maxIntensity = light.intensity;
        OnWorkEnd();
    }

    private void Update()
    {
        if (isWorking)
        {
            if (warmup < warmupTime)
            {
                warmup += Time.deltaTime;
            }
        }
        else
        {
            if (warmup > 0)
            {
                warmup = Mathf.Max(0f, warmup - Time.deltaTime);
            }
        }
        light.intensity = Mathf.Lerp(0f, maxIntensity, warmup / warmupTime);
    }

    public void OnWorkStart()
    {
        isWorking = true;
        smoke.Play();
    }

    public void OnWorkEnd()
    {
        isWorking = false;
        smoke.Stop();
    }
}

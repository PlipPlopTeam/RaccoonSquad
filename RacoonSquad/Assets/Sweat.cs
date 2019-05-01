using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweat : MonoBehaviour
{
    public Transform sweatOrigin;
    ParticleSystem particle;

    [Header("Maximum Effort")]
    int dropCount = 20;
    float minburstInterval = 0.5f;
    float maxBurstInterval = 1.5f;

    void Awake()
    {
        if(sweatOrigin == null) sweatOrigin = transform;
        particle = Instantiate(Library.instance.sweatParticle, sweatOrigin).GetComponent<ParticleSystem>();

        particle.gameObject.transform.localPosition = Vector3.zero;
        particle.gameObject.transform.localEulerAngles = Vector3.zero;

        if(particle == null) Destroy(this);

        Set(0f);
    }

    public void Activate()
    {
        //particle.emission.SetBursts( new ParticleSystem.Burst[] { new ParticleSystem.Burst(10, 0) } );
        //article.Play();
        particle.Play();
    }

    public void Desactivate()
    {
        particle.Stop();
    }

    public void Set(float effort)
    {
        ParticleSystem.Burst b = particle.emission.GetBurst(0);

        b.count = Mathf.Floor(effort * dropCount);
        b.repeatInterval = maxBurstInterval - (effort * (maxBurstInterval - minburstInterval));

        Debug.Log(b.count + " " + b.repeatInterval);

        particle.emission.SetBurst(0, b);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweat : MonoBehaviour
{
    public Transform sweatOrigin;
    ParticleSystem particle;

    [Header("Effort Settings")]
    public int maxDropCount = 20;
    public float minBurstInterval = 0.25f;
    public float maxBurstInterval = 1f;

    void Awake()
    {
        if(sweatOrigin == null) sweatOrigin = transform;
        particle = Instantiate(Library.instance.sweatParticle, sweatOrigin).GetComponent<ParticleSystem>();

        particle.gameObject.transform.localPosition = Vector3.zero;
        particle.transform.up = Vector3.up;

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
        b.count = Mathf.Floor(effort * maxDropCount);
        b.repeatInterval = maxBurstInterval - (effort * (maxBurstInterval - minBurstInterval));
        particle.emission.SetBurst(0, b);
    }
}
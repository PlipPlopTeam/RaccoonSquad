using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public Renderer MeshRenderer;
    public ParticleSystem particle;
    Material mat;


    public AnimationCurve curve;

    void Awake()
    {
        mat = Instantiate(MeshRenderer.material);
        MeshRenderer.material = mat;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Rigidbody>())
        {
            mat.SetVector("_ShakeDirection", new Vector4(Random.Range(-2f, 2f), 0f,Random.Range(-2f, 2f),0));
            particle.Play();
            StartCoroutine(Shake());
            SoundPlayer.PlayAtPosition("fb_walking_bushes", transform.position, 0.3f, false);
        }
    }

    IEnumerator Shake()
    {
        float _y = 0;
        while (_y < 1)
        {
            mat.SetFloat("_ShakeEffect", curve.Evaluate(_y));
            _y += Time.deltaTime;
            yield return null;
        }
    }

    void OnDestroy()
    {
        Destroy(mat);
    }
}

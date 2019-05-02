using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTest : MonoBehaviour
{
    public Renderer rend;

    private Material mat;
    public AnimationCurve curve;


    private void Start()
    {
        mat = rend.material;
        rend.material = mat;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            
            mat.SetVector("_ShakeDirection", new Vector4(1,0,0,0));
            StartCoroutine(Shake());
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
}

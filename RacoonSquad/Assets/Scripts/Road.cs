using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public GameObject car;

    public Transform[] positions;

    public float maxTimer;

    public float speed;

    public Renderer rend;

    public Material mat;

    public Material coloredMat;

    public Color[] colours;
    
    void Start()
    {
        coloredMat = mat;        
        
        StartCoroutine(CarTrip());
    }


    IEnumerator CarTrip()
    {
        
        coloredMat.SetColor("_BaseColor", colours[Random.Range(0, colours.Length)]);
        rend.material = coloredMat;
        
        float _y = 0;
        Vector3 _start;
        Vector3 _end;

        float _speed = Random.Range(speed * 2, speed / 2);
        
        
        if (Random.Range(0f, 1f) > .5f)
        {
            _start = positions[0].position;
            _end = positions[1].position;
        }
        else
        {
            _start = positions[1].position;
            _end = positions[0].position;
        }


        while (_y<1)
        {
            car.transform.position = Vector3.Lerp(_start, _end, _y);

            _y += Time.deltaTime * _speed;
            yield return null;
        }

        float _time = Random.Range(maxTimer / 2, maxTimer);
        
        yield return new WaitForSecondsRealtime(_time);
        
        StartCoroutine(CarTrip());
    }

}

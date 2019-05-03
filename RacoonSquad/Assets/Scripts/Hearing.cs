using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    [Header("Settings")]
    public float range = 5f;
    [Range(0f, 1f)]public float multiplier = 1f;

    public event System.Action<Vector3> heard;

    void Start()
    {
        GameManager.instance.level.soundAt += (Vector3 position) => { this.TryHeard(position); };
    }

    public void TryHeard(Vector3 position)
    {
        if(Vector3.Distance(transform.position, position) * multiplier <= range) OnHeard(position);
    }

    void OnHeard(Vector3 position)
    {
        heard.Invoke(position);
    }
}
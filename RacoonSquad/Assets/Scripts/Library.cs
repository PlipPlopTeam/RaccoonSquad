using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour
{
    public static Library instance;

    [Header("Prefabs")]
    public GameObject racoonPrefab;
    public GameObject exclamationMarkPrefab;

    [Header("Particles")]
    public GameObject sweatParticle;

    void Awake()
    {
        instance = this;
    }
}

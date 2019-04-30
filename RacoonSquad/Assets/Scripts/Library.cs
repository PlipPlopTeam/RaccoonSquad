using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour
{
    public static Library instance;

    [Header("Prefabs")]
    public GameObject racoonPrefab;
    public GameObject exclamationMarkPrefab;

    void Awake()
    {
        instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour
{
    public static Library instance;

    [Header("Scenes")]
    public Object lobbyScene;
    public List<Object> levels;

    [Header("Prefabs")]
    public GameObject racoonPrefab;
    public GameObject exclamationMarkPrefab;

    [Header("Particles")]
    public GameObject sweatParticle;
    public GameObject hitFX;
    public List<Color> playersColors;

    [Header("Materials")]
    public Material boardMaterial;

    [Header("Sounds")]
    public List<SoundPlayer.Sound> sounds;

    void Awake()
    {
        instance = this;
    }
}

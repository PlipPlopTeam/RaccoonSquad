using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject racoonPrefab;
    public int playerCount;

    void Start()
    {   
        for(int i = 0; i < playerCount; i++)
        {
            PlayerController pc = Instantiate(racoonPrefab).GetComponent<PlayerController>();
            pc.gameObject.name = "Racoon_" + i;

            PlayerInputs pi = gameObject.AddComponent<PlayerInputs>();
            pi.LoadGamepad(i);
            pc.controller = pi;
        }   
    }
}

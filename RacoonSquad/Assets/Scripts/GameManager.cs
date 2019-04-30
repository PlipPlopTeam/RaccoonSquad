using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject racoonPrefab;
    public bool lobby = false;
    public int playerCount;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {   
        if(lobby)
        {
            InterfaceManager.instance.CreateLobby();
        }
        else
        {
            DebugSpawnControllers();
        }
    }

    public void SpawnControllers(PlayerIndex[] players)
    {
        for(int i = 0; i < players.Length; i++)
        {
            PlayerController pc = Instantiate(racoonPrefab).GetComponent<PlayerController>();
            pc.gameObject.name = "Racoon_" + players[i];

            pc.index = players[i];
        }   
    }

    public void DebugSpawnControllers()
    {
        for(int i = 0; i < playerCount; i++)
        {
            PlayerController pc = Instantiate(racoonPrefab).GetComponent<PlayerController>();
            pc.gameObject.name = "Racoon_" + i;
            pc.index = (PlayerIndex)i;
        }   
    }
}

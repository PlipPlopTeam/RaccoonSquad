using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool lobby = false;
    public int playerCount;
    public LevelMaster level;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {   
        if(lobby)
        {
            try {
                InterfaceManager.instance.CreateLobby();
            }
            catch (System.Exception e){
                Debug.LogWarning("Could not create the lobby :\n" + e.ToString());
            }
        }
        else
        {
            // Initialize goal score etc...
            level = new LevelMaster();
            DebugSpawnControllers();
        }
    }

    public void SpawnControllers(PlayerIndex[] players)
    {
        for(int i = 0; i < players.Length; i++)
        {
            PlayerController pc = Instantiate(Library.instance.racoonPrefab).GetComponent<PlayerController>();
            pc.gameObject.name = "Racoon_" + players[i];

            pc.index = players[i];
        }   
    }

    public void DebugSpawnControllers()
    {
        for(int i = 0; i < playerCount; i++)
        {
            PlayerController pc = Instantiate(Library.instance.racoonPrefab).GetComponent<PlayerController>();
            pc.gameObject.name = "Racoon_" + i;
            pc.index = (PlayerIndex)i;
        }   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Lobby : MonoBehaviour
{
    public int minPlayers = 2;

    List<PlayerIndex> players = new List<PlayerIndex>();

    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            PlayerIndex controllerIndex = (PlayerIndex)i;
            if(!players.Contains(controllerIndex) && GamePad.GetState(controllerIndex).Buttons.A == ButtonState.Pressed)
            {
                Join(controllerIndex);
            }
        }

        /*
        if(players.Count > 0 && GamePad.GetState(players[0]).Buttons.Start == ButtonState.Pressed)
        {
            StartGame();
        }
        */
    }

    void Join(PlayerIndex controllerIndex)
    {
        players.Add(controllerIndex);
        foreach(var spawn in GameObject.FindObjectsOfType<PlayerSpawn>()) {
            if (spawn.playerIndex == controllerIndex) {
                GameManager.instance.SpawnPlayer(controllerIndex, spawn.transform.position);
                return;
            }
        }
        GameManager.instance.SpawnPlayer(controllerIndex);
    }

    void StartGame()
    {
        // Loading first level
        SceneManager.LoadSceneAsync(Library.instance.levels[0].name);
        Destroy(gameObject);
    }

    public bool IsReady()
    {
        return players.Count >= minPlayers;
    }

    public int GetNeededPlayers()
    {
        return minPlayers - players.Count;
    }
}

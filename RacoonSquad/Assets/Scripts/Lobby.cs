using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Lobby : MonoBehaviour
{
    public int minPlayers = 2;

    List<GameManager.Player> GetPlayers()
    {
        return GameManager.instance.GetPlayers();
    }

    void Start()
    {
        float amplitude = 5f;
        foreach(var cosmetic in Library.instance.cosmetics) {
            //Spawn every hat
            Instantiate(cosmetic, 
                new Vector3(Random.value* amplitude - amplitude/2, 0.3f, Random.value*amplitude - amplitude/2), Quaternion.identity);
        }
    }

    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            PlayerIndex controllerIndex = (PlayerIndex)i;
            if (
                !GameManager.instance.PlayerExists(controllerIndex) &&
                GamePad.GetState(controllerIndex).Buttons.A == ButtonState.Pressed
            )
            {
                Join(controllerIndex);
            }
        }
    }

    void Join(PlayerIndex controllerIndex)
    {
        // Add player to the game
        GameManager.instance.AddPlayer(new GameManager.Player() { index = controllerIndex });

        // Spawn it
        foreach(var spawn in GameObject.FindObjectsOfType<PlayerSpawn>()) {
            if (spawn.playerIndex == controllerIndex) {
                GameManager.instance.SpawnPlayer(controllerIndex, spawn.transform.position);
                return;
            }
        }

        // If no spawns, spawn it in the middle of the map
        GameManager.instance.SpawnPlayer(controllerIndex);
    }

    public bool IsReady()
    {
        return GetPlayers().Count >= minPlayers;
    }

    public int GetNeededPlayers()
    {
        return minPlayers - GetPlayers().Count;
    }
}

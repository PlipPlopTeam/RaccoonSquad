using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Lobby : MonoBehaviour
{
    [Header("Referencies")]
    public Transform holder;
    public Image[] images;
    public Text[] texts;

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

        if(players.Count > 0 && GamePad.GetState(players[0]).Buttons.Start == ButtonState.Pressed)
        {
            StartGame();
        }
    }

    void Join(PlayerIndex controllerIndex)
    {
        Debug.Log("Controller " + controllerIndex + " has join the game as player " + (players.Count + 1));
        images[players.Count].enabled = false;
        texts[players.Count].text = "Controller " + controllerIndex;
        players.Add(controllerIndex);
    }

    void StartGame()
    {
        GameManager.instance.SpawnControllers(players.ToArray());

        Destroy(gameObject);
    }
}

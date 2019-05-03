using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hider : MonoBehaviour
{
    List<PlayerController> players = new List<PlayerController>();
    
    void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if(pc != null && !players.Contains(pc)) AddPlayer(pc);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if(pc != null) RemovePlayer(pc);
    }

    void AddPlayer(PlayerController pc)
    {
        players.Add(pc);
        pc.hidden = true;
    }

    void RemovePlayer(PlayerController pc)
    {
        players.Remove(pc);
        pc.hidden = false;
    }

    void Update()
    {
        HideTick();
    }

    void HideTick()
    {
        foreach(PlayerController pc in players)
        {
            pc.hidden = true;
        }
    }
}

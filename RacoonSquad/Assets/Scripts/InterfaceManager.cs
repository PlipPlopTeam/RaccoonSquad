using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager instance;
    public GameObject lobbyPrefab;

    void Awake()
    {
        instance = this;
    }

    public void CreateLobby()
    {
        Instantiate(lobbyPrefab, transform);
    }
}

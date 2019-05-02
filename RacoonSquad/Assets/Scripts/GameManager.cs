using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    //////////////////////////
    ///
    /// Cheats
    /// 

    private int bufferLength = 20;
    private class Cheats : Dictionary<string, System.Action> { }
    private string keyBuffer = string.Empty;
    private Cheats cheats = new Cheats {
        // Everything becomes grababble
        {"GRABALL",
            delegate { foreach (var obj in FindObjectsOfType<Prop>()){ obj.gameObject.AddComponent<Grabbable>();}}
        },

        // Resets the game
        {"DEJAVU",
            delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
        }
    };

    public static KeyCode KeyDown(bool getDef=false)
    {
        KeyCode def = KeyCode.Break;
        if (getDef) return def;
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
            if ((int)key > 330) break;
            if (Input.GetKeyDown(key)) return key;
        }
        return def;
    }

    private void LateUpdate()
    {
        if (Input.anyKeyDown) {
            var key = KeyDown();
            if (key != KeyDown(true)) {
                keyBuffer += key.ToString();
                while (keyBuffer.Length > bufferLength) keyBuffer = keyBuffer.Remove(0,1);
                DetectCheat();
            }
        }
    }

    private void DetectCheat()
    {
        foreach(string cheatCode in cheats.Keys) {
            if (keyBuffer.ToUpper().EndsWith(cheatCode)) {
                cheats[cheatCode].Invoke();
                print("!");
                return;
            }
        }
    }

}

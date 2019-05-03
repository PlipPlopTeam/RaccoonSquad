using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //////////////////////////
    ///
    /// Defs
    /// 

    public bool lobby = false;
    public GameObject lobbyPrefab;
    public int playerCount;
    public LevelMaster level;

    int currentLevel = -1;
    List<Player> players = new List<Player>();

    [System.Serializable]
    public class Player {
        public PlayerIndex index;
        public int cosmetic;

    }


    //////////////////////////
    ///
    /// MonoBehaviour high level loop
    /// 

    void Awake()
    {
        // No dupes
        if (FindObjectsOfType<GameManager>().Length > 1) {
            DestroyImmediate(this.gameObject);
            return;
        }

        // Setting up the static & persistent parameters
        DontDestroyOnLoad(this);
        instance = this;

        // Onload
        SceneManager.sceneLoaded += delegate {
            if (lobby) {
                try {
                    Instantiate(lobbyPrefab, transform);
                }
                catch (System.Exception e) {
                    Debug.LogWarning("Could not create the lobby :\n" + e.ToString());
                }
            }
            else {
                // Initialize goal score etc...
                level = new LevelMaster();
                if (players.Count > 0) {
                    SpawnPlayers();
                }
                else {
                    DebugSpawnControllers();
                }
            }
        };
    }


    //////////////////////////
    ///
    /// Level flow
    /// 

    public void NextLevel()
    {
        lobby = false;
        currentLevel++;
        SceneManager.LoadSceneAsync(Library.instance.levels[currentLevel].name);
    }

    public void GoToLobby()
    {
        players.Clear();
        SceneManager.LoadSceneAsync(Library.instance.lobbyScene.name);
        lobby = true;
    }

    //////////////////////////
    ///
    /// Player management
    /// 

    public bool PlayerExists(PlayerIndex index)
    {
        return players.Find(o => o.index == index) != null;
    }

    public List<Player> GetPlayers()
    {
        return new List<Player>(players.ToArray());
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }
    
    public void UpdatePlayer(PlayerIndex index, Player player)
    {
        players[players.FindIndex(o => o.index == index)] = player;
    }
    public void GameOver()
    {
        foreach (PlayerController pc in FindObjectsOfType<PlayerController>()) pc.Die();
    }


    public void SpawnPlayers()
    {
        for(int i = 0; i < players.Count; i++) {

            var pc = SpawnPlayer(players[i].index);
            if (players[i].cosmetic >= 0) {
                var cos = Instantiate(Library.instance.cosmetics[players[i].cosmetic], pc.transform);
                pc.Wear(cos.GetComponent<Grabbable>());
            }
        }   
    }

    public PlayerController SpawnPlayer(PlayerIndex player, Vector3 position)
    {

        PlayerController pc = Instantiate(Library.instance.racoonPrefab, position, Quaternion.identity).GetComponent<PlayerController>();
        pc.gameObject.name = "Racoon_" + player;
        pc.index = player;
        pc.ReloadColor();

        return pc;
    }

    public PlayerController SpawnPlayer(PlayerIndex player)
    {
        foreach (var spawn in FindObjectsOfType<PlayerSpawn>()) {
            if (spawn.playerIndex == player) {
                return SpawnPlayer(player, spawn.transform.position);
            }
        }

        return SpawnPlayer(player, new Vector3());
    }

    //////////////////////////
    ///
    /// DEBUG
    /// 
    public void DebugSpawnControllers()
    {
        for(int i = 0; i < playerCount; i++)
        {
            SpawnPlayer((PlayerIndex)i);
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
        },

        {"MUSIC", delegate { SoundPlayer.Play("debug_music"); }},
        {"STOP", delegate { SoundPlayer.StopEverySound(); }},
        {"PSOUND", delegate { SoundPlayer.Play("debug_sound"); }},
        {"RPITCH", delegate { SoundPlayer.PlayWithRandomPitch("debug_sound"); }},
        {"LOOPME", delegate { SoundPlayer.PlaySoundAttached("debug_sound_looping", FindObjectOfType<PlayerController>().transform); }},
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

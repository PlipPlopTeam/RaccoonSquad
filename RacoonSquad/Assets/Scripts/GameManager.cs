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
            DestroyImmediate(this);
            return;
        }

        // Setting up the static & persistent parameters
        DontDestroyOnLoad(this);
        instance = this;
    }

    void Start()
    {
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
            SpawnPlayers();
        }
    }

    //////////////////////////
    ///
    /// Level flow
    /// 

    public void NextLevel()
    {
        currentLevel++;
        SceneManager.LoadSceneAsync(Library.instance.levels[currentLevel].name);
    }

    public void GoToLobby()
    {

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
        players.RemoveAll(o => o.index == index);
        AddPlayer(player);
    }
    public void GameOver()
    {
        foreach (PlayerController pc in FindObjectsOfType<PlayerController>()) pc.Die();
    }


    public void SpawnPlayers()
    {
        for(int i = 0; i < players.Count; i++)
        {
            SpawnPlayer(players[i].index);
        }   
    }

    public void SpawnPlayer(PlayerIndex player, Vector3 position)
    {

        PlayerController pc = Instantiate(Library.instance.racoonPrefab, position, Quaternion.identity).GetComponent<PlayerController>();
        pc.gameObject.name = "Racoon_" + player;

        pc.index = player;
    }

    public void SpawnPlayer(PlayerIndex player)
    {
        SpawnPlayer(player, new Vector3());
    }

    //////////////////////////
    ///
    /// DEBUG
    /// 
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

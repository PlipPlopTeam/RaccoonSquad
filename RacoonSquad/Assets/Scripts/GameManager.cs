using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
    public LevelMaster previousLevel;

    int currentLevel = -1;
    List<Player> players = new List<Player>();

    [System.Serializable]
    public class Player {
        public PlayerIndex index;
        public int cosmetic = -1;

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
                previousLevel = level;

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

    public void SpawnHuman()
    {
        // Get reference to point
        List<Transform> waypoints = new List<Transform>();
        List<Transform> spawnpoints = new List<Transform>();
        foreach(Waypoint wp in  FindObjectsOfType<Waypoint>()) waypoints.Add(wp.transform);
        foreach(HumanSpawn hs in  FindObjectsOfType<HumanSpawn>()) spawnpoints.Add(hs.transform);

        // Spawn the actor
        HumanBehavior human = Instantiate(Library.instance.humanPrefab).GetComponent<HumanBehavior>();

        // If some spawnPoints has been found
        if(spawnpoints.Count > 0) human.transform.position = spawnpoints[Random.Range(0, spawnpoints.Count)].position;

        // Apply the path to the actor
        human.paths = waypoints;
    }

    //////////////////////////
    ///
    /// Level flow
    /// 

    public void NextLevel()
    {
        try {
            lobby = false;
            currentLevel++;
            SceneManager.LoadSceneAsync(
                Library.instance.levels[currentLevel]
            );
        }
        
        // This is insane
        catch {
            GoToLobby();
        }
    }

    public void GoToLobby()
    {
        currentLevel = -1;
        players.Clear();
        SceneManager.LoadScene(Library.instance.lobbyScene);
        lobby = true;
    }

    public void GameOver()
    {
        Instantiate(Library.instance.gameOverPrefab);
    }

    public void Win()
    {
        Instantiate(Library.instance.winPrefab);
    }

    public void GoToWinScene()
    {
        SceneManager.LoadScene(Library.instance.winScene);
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

    public void SpawnPlayers()
    {
        for(int i = 0; i < players.Count; i++) {

            var pc = SpawnPlayer(players[i].index);
            if (players[i].cosmetic >= 0) {
                pc.ReloadColor();
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

        {"MULTICLONAGE",
            delegate { for(int i = 0; i < 10; i++) GameManager.instance.SpawnHuman(); }
        },

        {"MUSIC", delegate { SoundPlayer.Play("debug_music"); }},
        {"STOP", delegate { SoundPlayer.StopEverySound(); }},
        {"PSOUND", delegate { SoundPlayer.Play("debug_sound"); }},
        {"RPITCH", delegate { SoundPlayer.PlayWithRandomPitch("debug_sound"); }},
        {"LOOPME", delegate { SoundPlayer.PlaySoundAttached("debug_sound_looping", FindObjectOfType<PlayerController>().transform); }},
        {"WIN", delegate { GameManager.instance.Win(); }},
        {"MHH", delegate{FindObjectOfType<Camera>().GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().active = true; }},
        {"PEW", delegate{FindObjectOfType<Camera>().GetComponent<PostProcessVolume>().profile.GetSetting<ChromaticAberration>().active = true; }},
        {"WII", delegate{FindObjectOfType<Camera>().GetComponent<PostProcessVolume>().profile.GetSetting<Bloom>().active = true; }},
        {"GRR", delegate{FindObjectOfType<Camera>().GetComponent<PostProcessVolume>().profile.GetSetting<Grain>().active = true; }},
        {"PIX", delegate { FindObjectOfType<Camera>().GetComponent<PostProcessLayer>().enabled = false; }}

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

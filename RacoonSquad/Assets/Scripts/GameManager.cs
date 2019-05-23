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

    public enum SceneType { Game, Lobby, Editor };
    public SceneType sceneType;
    public GameObject lobbyPrefab;
    public int playerCount;
    public LevelMaster level;
    public LevelMaster previousLevel;

    List<string> completedLevels = new List<string>();
    int currentLevel = -1;
    List<Player> players = new List<Player>();
    bool nextLoadIsXml = false;
    string nextLevelName = "";

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
            switch (sceneType) {
                case SceneType.Lobby:
                    try {
                        Instantiate(lobbyPrefab, transform);
                    }
                    catch (System.Exception e) {
                        Debug.LogWarning("Could not create the lobby :\n" + e.ToString());
                    }
                    break;

                case SceneType.Game:
                    previousLevel = level;
                    if (nextLoadIsXml) {
                        nextLoadIsXml = false;
                        try {
                            LevelEditor.LoadLevel(nextLevelName);
                        }
                        catch {
                            GoToLobby();
                        }
                    }

                    // Initialize goal score etc...
                    level = new LevelMaster();
                    if (players.Count > 0) {
                        SpawnPlayers();
                    }
                    else {
                        DebugSpawnControllers();
                    }

                    break;

                case SceneType.Editor:
                    try
                    {
                        AddPlayer(new Player() { index = (PlayerIndex)0 });
                        SpawnControllableHuman((PlayerIndex)0, new Vector3(1f+Random.value*2f, 0f, 1f+Random.value*2f));
                        Instantiate(Library.instance.editorPrefab, transform);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning("Could not create the editor :\n" + e.ToString());
                    }
                    break;
            }
        };
    }

    public void SpawnHuman()
    {
        // Get reference to point
        List<Transform> spawnpoints = new List<Transform>();
        foreach(HumanSpawn hs in  FindObjectsOfType<HumanSpawn>()) spawnpoints.Add(hs.transform);
        // Spawn the actor
        HumanBehavior human = Instantiate(Library.instance.humanPrefab).GetComponent<HumanBehavior>();
        // If some spawnPoints has been found
        if(spawnpoints.Count > 0) human.transform.position = spawnpoints[Random.Range(0, spawnpoints.Count)].position;
    }

    //////////////////////////
    ///
    /// Level flow
    /// 

    public void NextLevel()
    {
        ClearStoredProps();
        sceneType = SceneType.Game;
        currentLevel++;

        if (currentLevel < Library.instance.levels.Count) {
            SceneManager.LoadSceneAsync(
                Library.instance.levels[currentLevel]
            );
        }
        else {
            try {
                nextLevelName = FindNextXLevel();
                SceneManager.LoadScene("LevelEditor");
                nextLoadIsXml = true;
            }
            catch {
                GoToLobby();
            }

        }
    }

    string FindNextXLevel()
    {
        var lvlName = "";
        foreach(var filename in System.IO.Directory.EnumerateFiles(Application.streamingAssetsPath + "/levels")) {
            if (completedLevels.Contains(filename)) continue;
            if (!filename.ToLower().EndsWith(".xml")) continue;
            lvlName = filename;
            break;
        }
        completedLevels.Add(lvlName);
        return lvlName;
        throw new System.Exception();
    }

    public void GoToLobby()
    {
        ClearStoredProps();

        currentLevel = -1;
        players.Clear();
        SceneManager.LoadScene(Library.instance.lobbyScene);
        sceneType = SceneType.Lobby;
    }

    public void GoToLevelEditor()
    {
        ClearStoredProps();

        currentLevel = -1;
        SceneManager.LoadScene(Library.instance.editorScene);
        sceneType = SceneType.Editor;
    }

    void ClearStoredProps()
    {
        for (int i = 0; i < GameManager.instance.transform.childCount; i++) {
            var child = GameManager.instance.transform.GetChild(i).gameObject.GetComponent<Prop>();
            if (child == null) {
                continue;
            }
            Destroy(GameManager.instance.transform.GetChild(i).gameObject);
        }
    }

    public void SaveLevelWindow()
    {
        if (sceneType == SceneType.Editor) {
            var editorMenu = FindObjectOfType<LevelEditor>();
            editorMenu.ShowSaveMenu();
        }
    }

    public void GameOver()
    {
        Instantiate(Library.instance.transitionPrefab).GetComponent<CircleFrame>().PlayFrameAnimation("GameOver");
    }

    public void Win()
    {
        foreach(var player in FindObjectsOfType<PlayerController>()) {
            player.MakeInvincible();
            player.Paralyze();
        }
        Instantiate(Library.instance.transitionPrefab).GetComponent<CircleFrame>().PlayFrameAnimation("Win");
    }

    public void GoToWinScene()
    {
        SceneManager.LoadScene(Library.instance.winScene);
    }

    public bool IsInLobby()
    {
        return SceneType.Lobby == sceneType;
    }

    public bool IsInGame()
    {
        return SceneType.Game == sceneType;
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

    public PlayerController SpawnPlayer(PlayerIndex player, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion())
    {

        PlayerController pc = Instantiate(Library.instance.racoonPrefab, position, rotation).GetComponent<PlayerController>();
        pc.gameObject.name = "Racoon_" + player;
        pc.index = player;
        pc.ReloadColor();
        return pc;
    }

    public PlayerController SpawnControllableHuman(PlayerIndex player, Vector3 position)
    {

        PlayerController pc = Instantiate(Library.instance.editorHumanPrefab, position, Quaternion.identity).GetComponent<PlayerController>();
        pc.gameObject.name = "DisguisedRacoon_" + player;
        pc.index = player;
        pc.ReloadColor();
        pc.carryCapacity = 1000f;

        return pc;
    }
    
    public PlayerController SpawnPlayer(PlayerIndex player)
    {
        foreach(var spawn in FindObjectsOfType<PlayerSpawn>())
        {
            if (spawn.playerIndex == player)
            {
                Quaternion rot = Quaternion.Euler(0f, spawn.transform.eulerAngles.y, 0f);
                return SpawnPlayer(player, spawn.transform.position, rot);
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
            AddPlayer(new Player() { index = (PlayerIndex)i });
        }
        SpawnPlayers();
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
        {"SCORE", delegate { if(GameManager.instance.level != null) GameManager.instance.level.AddScore(100); }},
        {"MUSIC", delegate { SoundPlayer.Play("debug_music"); }},
        {"KANOZIEV", delegate { SoundPlayer.Play("debug_music_2"); }},
        {"STOP", delegate { SoundPlayer.StopEverySound(); }},
        {"PSOUND", delegate { SoundPlayer.Play("debug_sound"); }},
        {"RPITCH", delegate { SoundPlayer.PlayWithRandomPitch("debug_sound"); }},
        {"LOOPME", delegate { SoundPlayer.PlaySoundAttached("debug_sound_looping", FindObjectOfType<PlayerController>().transform); }},
        {"WIN", delegate { GameManager.instance.Win(); }},
        {"MHH", delegate{ Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().active = true; }},
        {"PEW", delegate{ Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<ChromaticAberration>().active = true; }},
        {"WII", delegate{ Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<Bloom>().active = true; }},
        {"GRR", delegate{ Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<Grain>().active = true; }},
        {"PIX", delegate { Camera.main.GetComponent<PostProcessLayer>().enabled = false; }},
        {"GARBAGE", delegate { Instantiate(Library.instance.props[Random.Range(0, Library.instance.props.Count-1)]); }},
        {"SAVE", delegate {FindObjectOfType<LevelEditor>().Save(); }},
        {"NEXTXML", delegate {
                instance.ClearStoredProps();
                instance.sceneType = SceneType.Game;
                instance.nextLevelName = instance.FindNextXLevel();
                SceneManager.LoadScene("LevelEditor");
                instance.nextLoadIsXml = true;
        }},
        {"EDITOR", delegate {
            instance.GoToLevelEditor();
        }  }

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

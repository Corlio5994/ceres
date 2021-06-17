using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static Player mainPlayer { get; private set; }
    public static bool showingUI { get; private set; } = false;
    public static LayerMask groundMask { get; private set; }
    public static LayerMask interactableMask { get; private set; }
    static Player playerPrefab;
    static GameManager instance;
    static Dictionary<int, Player> players = new Dictionary<int, Player> ();

    [SerializeField] Player _playerPrefab;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _interactableMask;

    void Awake () {
        if (instance != null)
            Destroy (gameObject);
        instance = this;

        playerPrefab = _playerPrefab;
        groundMask = _groundMask;
        interactableMask = _interactableMask;
    }

    void Start () {
        if (StateManager.singlePlayer) {
            SpawnPlayer (Vector3.up, Quaternion.identity);
        } else if (StateManager.client) {
            PacketSender.PlayerDataRequest ();
            PacketSender.ItemPickupDataRequest();
            PacketSender.BankDataRequest();
        } else {
            #if UNITY_SERVER || UNITY_EDITOR
            GameServer.Server.Start ();

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Constants.ticksPerSecond;
            #endif
        }
    }

    void LateUpdate () {
        showingUI = ShowingUI ();
    }

    private static bool ShowingUI () {
        return InventoryUI.shown || EscapeMenuUI.shown || ContainerUI.shown || Chat.typing || Console.shown;
    }

    public static Player SpawnPlayer (Vector3 position, Quaternion rotation, int clientID = -1) {
        Console.Log($"Spawning player: {clientID}");

        Player player = Instantiate(playerPrefab, position, rotation);
        players.Add (clientID, player);

        if (clientID == -1) {
            mainPlayer = player;
            CameraController.SetTarget (player.transform);
        }

        return player;
    }

    public static void DestroyPlayer (int clientID) {
        Destroy (players[clientID].gameObject);
        players.Remove (clientID);
    }

    public static Player GetPlayer (int clientID) {
        if (players.ContainsKey (clientID))
            return players[clientID];
        return null;
    }

    public static void Logout () {
        players.Clear ();
        mainPlayer = null;
    }

    public void OnApplicationQuit () {
        #if UNITY_SERVER || UNITY_EDITOR
        if (StateManager.server) {
            GameServer.Server.Stop ();
        }
        #endif
    }

    public static void Quit () {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static Player player { get; private set; }
    public static bool showingUI { get; private set; } = false;
    public static LayerMask groundMask { get; private set; }
    public static LayerMask interactableMask { get; private set; }
    static Entity playerPrefab;
    static Entity otherPlayerPrefab;
    static GameManager instance;
    static Dictionary<int, Person> otherPlayers = new Dictionary<int, Person> ();

    [SerializeField] Player _playerPrefab;
    [SerializeField] Person _otherPlayerPrefab;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _interactableMask;

    void Awake () {
        if (instance != null)
            Destroy (gameObject);
        instance = this;

        playerPrefab = _playerPrefab;
        otherPlayerPrefab = _otherPlayerPrefab;
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
            GameServer.Server.Start ();

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Constants.ticksPerSecond;
        }
    }

    void LateUpdate () {
        showingUI = ShowingUI ();
    }

    private static bool ShowingUI () {
        return InventoryUI.shown || EscapeMenuUI.shown || ContainerUI.shown || Chat.typing || Console.shown;
    }

    public static Entity SpawnPlayer (Vector3 position, Quaternion rotation, int clientID = -1) {
        Console.Log($"Spawning player: {clientID}");
        Entity newPlayer = Instantiate (clientID == -1 ? playerPrefab : otherPlayerPrefab, position, rotation);

        if (clientID != -1) {
            otherPlayers.Add (clientID, (Person) newPlayer);
        } else {
            player = (Player) newPlayer;
            CameraController.SetTarget (player.transform);
        }

        return newPlayer;
    }

    public static void DestroyPlayer (int clientID) {
        Destroy (otherPlayers[clientID].gameObject);
        otherPlayers.Remove (clientID);
    }

    public static Person GetPlayer (int clientID) {
        return otherPlayers[clientID];
    }

    public static void Logout () {
        otherPlayers.Clear ();
        player = null;
    }

    public void OnApplicationQuit () {
        if (StateManager.server) {
            GameServer.Server.Stop ();
        }
    }

    public static void Quit () {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private static Entity playerPrefab;
    private static Entity otherPlayerPrefab;
    private static GameManager instance;
    private static Dictionary<int, OtherPlayer> otherPlayers = new Dictionary<int, OtherPlayer> ();
    public static Player player { get; private set; }
    public static LayerMask groundMask { get; private set; }

    [SerializeField] private bool singlePlayer = false;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private OtherPlayer _otherPlayerPrefab;

    void Awake () {
        if (instance != null)
            Destroy (gameObject);
        instance = this;

        playerPrefab = _playerPrefab;
        otherPlayerPrefab = _otherPlayerPrefab;
        groundMask = _groundMask;

        if (playerPrefab == null) Debug.LogError ("Player prefab is equal to null");
    }

    void Start () {
        if (singlePlayer && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) {
            SpawnPlayer (Vector3.up, Quaternion.identity);
        } else if (Client.connected) {
            PacketSender.PlayerDataRequest ();
        } else {
            GameServer.Server.Start ();

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Constants.ticksPerSecond;
        }
    }

    public static void LoadScene (string scene) {
        SceneManager.LoadSceneAsync (scene);
    }

    public static Entity SpawnPlayer (Vector3 position, Quaternion rotation, int clientID = -1) {
        Entity newPlayer = Instantiate (clientID == -1 ? playerPrefab : otherPlayerPrefab, position, rotation);

        if (clientID != -1) {
            otherPlayers.Add (clientID, (OtherPlayer) newPlayer);
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

    public static OtherPlayer GetPlayer (int clientID) {
        return otherPlayers[clientID];
    }

    public static void Logout () {
        otherPlayers.Clear ();
        player = null;
    }

    public void OnApplicationQuit () {
        if (GameServer.Server.active) {
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
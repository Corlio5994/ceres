using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class StateManager : MonoBehaviour {
    public static bool Client { get; private set; } = false;
    public static bool Server { get; private set; } = false;
    public static bool SinglePlayer { get; private set; } = false;

    [SerializeField] private bool _singlePlayer;

    public static void LoadScene (string name) {
        SceneManager.LoadScene (name);
    }

    public static void OnConnect () {
        Client = true;
        Server = false;
        SinglePlayer = false;
    }

    void Awake () {
        bool headless = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null; // Whether the server was built without graphics
        Server = (headless || !_singlePlayer) && !Client; // If we are headless or in Editor and singlePlayer is ticked, and Client isn't connected 
        SinglePlayer = !(Server || Client); // If we aren't Server or Client, we are SinglePlayer
    }
}
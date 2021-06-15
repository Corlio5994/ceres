using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class StateManager : MonoBehaviour {
    public static bool client { get; private set; } = false;
    public static bool server { get; private set; } = false;
    public static bool singlePlayer { get; private set; } = false;

    [SerializeField] bool _singlePlayer;

    public static void LoadScene (string name) {
        SceneManager.LoadScene (name);
    }

    public static void OnConnect () {
        client = true;
        server = false;
        singlePlayer = false;
    }

    void Awake () {
        bool headless = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null; // Whether the server was built without graphics
        server = (headless || !_singlePlayer) && !client; // If we are headless or in Editor and singlePlayer is ticked, and Client isn't connected 
        singlePlayer = !(server || client); // If we aren't Server or Client, we are SinglePlayer
    }
}
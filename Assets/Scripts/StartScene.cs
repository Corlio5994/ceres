using UnityEngine;
using UnityEngine.Rendering;

public class StartScene : MonoBehaviour {
    private enum ClientType {
        Server,
        Client
    }

    [SerializeField] private ClientType clientType;

    void Start () {
        Application.targetFrameRate = 60; // TODO: Load from settings
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null || clientType == ClientType.Server) {
            Console.Log("Preparing to launch");
            GameManager.LoadScene ("World");
        } else {
            GameManager.LoadScene ("Main Menu");
        }
    }
}
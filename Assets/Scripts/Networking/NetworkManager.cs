using UnityEngine;

public class NetworkManager : MonoBehaviour {
    void Start() {
        Client.Connect();
        UI.ShowLoadingSpinner();
    }

    void OnApplicationQuit() {
        Client.Disconnect();
    }
}
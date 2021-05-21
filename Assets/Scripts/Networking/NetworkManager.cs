using UnityEngine;

public class NetworkManager : MonoBehaviour {
    void Start() {
        if (Client.Connect())
            MainMenuUI.ShowLoadingSpinner();
        else
            MainMenuUI.ShowMainMenuPanel();
    }

    void OnApplicationQuit() {
        Client.Disconnect();
    }
}
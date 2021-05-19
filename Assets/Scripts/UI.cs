using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class UI : MonoBehaviour {
    private static GameObject mainMenuPanel;
    private static GameObject credentialPanel;
    private static GameObject loadingSpinner;
    private static TMP_InputField usernameInput;
    private static TMP_InputField passwordInput;

    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _credentialsPanel;
    [SerializeField] private GameObject _loadingSpinner;
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _passwordInput;

    void Awake () {
        mainMenuPanel = _mainMenuPanel;
        credentialPanel = _credentialsPanel;
        loadingSpinner = _loadingSpinner;

        usernameInput = _usernameInput;
        passwordInput = _passwordInput;
    }

    void Start () {
        DontDestroyOnLoad (gameObject);
    }

    public static void ShowMainMenuPanel () {
        mainMenuPanel.SetActive (true);
        credentialPanel.SetActive (false);
        loadingSpinner.SetActive (false);
    }

    public static void ShowCredentialsPanel () {
        mainMenuPanel.SetActive (false);
        credentialPanel.SetActive (true);
        loadingSpinner.SetActive (false);
    }

    public static void ShowLoadingSpinner () {
        mainMenuPanel.SetActive (false);
        credentialPanel.SetActive (false);
        loadingSpinner.SetActive (true);
    }

    public static void Login () {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (username != "" && password != "") {
            ShowLoadingSpinner ();
            PacketSender.Login (username, password);
        }
    }

    public static void Quit () {
        Application.Quit ();
    }
}
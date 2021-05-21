using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class MainMenuUI : MonoBehaviour {
    private static GameObject mainMenuPanel;
    private static GameObject credentialPanel;
    private static GameObject loadingSpinner;
    private static TMP_InputField usernameInput;
    private static TMP_InputField passwordInput;
    private static TMP_Text titleText;
    private static TMP_Text loginText;

    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _credentialsPanel;
    [SerializeField] private GameObject _loadingSpinner;
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _loginText;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private float maximumBobbing = 20f;
    [SerializeField] private float bobbingSpeed = 2f;

    void Awake () {
        mainMenuPanel = _mainMenuPanel;
        credentialPanel = _credentialsPanel;
        loadingSpinner = _loadingSpinner;

        usernameInput = _usernameInput;
        passwordInput = _passwordInput;

        titleText = _titleText;
        loginText = _loginText;
        versionText.text = Constants.version;
    }

    void Update () {
        titleText.margin = new Vector4 (titleText.margin.x, Mathf.Sin (Time.time * bobbingSpeed) * maximumBobbing, titleText.margin.z, titleText.margin.w);
        loginText.margin = titleText.margin;

        if (Input.GetKeyDown (KeyCode.Tab)) {
            if (usernameInput.isFocused) {
                usernameInput.DeactivateInputField();
                passwordInput.ActivateInputField ();
            } else {
                passwordInput.DeactivateInputField();
                usernameInput.ActivateInputField ();
            }
        }

        if (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return)) {
            Login ();
        }
    }

    public static void ShowMainMenuPanel () {
        mainMenuPanel.SetActive (true);
        credentialPanel.SetActive (false);
        loadingSpinner.SetActive (false);
    }

    public static void ShowCredentialsPanel () {
        usernameInput.text = "";
        passwordInput.text = "";
        usernameInput.ActivateInputField ();

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
        GameManager.Quit ();
    }
}
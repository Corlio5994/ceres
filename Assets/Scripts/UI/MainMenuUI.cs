using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class MainMenuUI : MonoBehaviour {
    static MainMenuUI instance;

    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject credentialsPanel;
    [SerializeField] GameObject loadingSpinner;
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text loginText;
    [SerializeField] TMP_Text versionText;
    [SerializeField] float maximumBobbing = 20f;
    [SerializeField] float bobbingSpeed = 2f;

    public static void ShowMainMenuPanel () {
        instance.mainMenuPanel.SetActive (true);
        instance.credentialsPanel.SetActive (false);
        instance.loadingSpinner.SetActive (false);
    }

    public static void ShowCredentialsPanel () {
        instance.usernameInput.text = "";
        instance.passwordInput.text = "";
        instance.usernameInput.ActivateInputField ();

        instance.mainMenuPanel.SetActive (false);
        instance.credentialsPanel.SetActive (true);
        instance.loadingSpinner.SetActive (false);
    }

    public static void ShowLoadingSpinner () {
        instance.mainMenuPanel.SetActive (false);
        instance.credentialsPanel.SetActive (false);
        instance.loadingSpinner.SetActive (true);
    }

    public static void Login () {
        string username = instance.usernameInput.text;
        string password = instance.passwordInput.text;

        if (username != "" && password != "") {
            ShowLoadingSpinner ();
            PacketSender.Login (username, password);
        }
    }

    public static void Quit () {
        GameManager.Quit ();
    }

    void Awake () {
        instance = this;
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuUI : MonoBehaviour {
    public static bool shown { get; private set; }
    private static RectTransform escapeMenu;
    [SerializeField] private RectTransform _escapeMenu;

    void Awake () {
        escapeMenu = _escapeMenu;
    }

    void Start () {
        Hide ();
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.Escape) && shown)
            Hide ();
        if (Input.GetKeyDown (KeyCode.Escape) && !GameManager.showingUI)
            Show ();
    }

    private void Hide () {
        escapeMenu.gameObject.SetActive (false);
        shown = false;
    }

    private void Show () {
        escapeMenu.gameObject.SetActive (true);
        shown = true;
    }

    public void Back () {
        Hide ();
    }

    public void Quit () {
        GameManager.Quit ();
    }

    public void Logout () {
        if (Client.connected)
            Client.Logout ();
        else
            GameManager.Quit ();
    }
}
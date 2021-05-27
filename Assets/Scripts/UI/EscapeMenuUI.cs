using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuUI : MonoBehaviour {
    public static bool active { get { return escapeMenu.gameObject.activeInHierarchy; } }
    private static RectTransform escapeMenu;
    [SerializeField] private RectTransform _escapeMenu;

    void Awake () {
        escapeMenu = _escapeMenu;
    }

    void Start () {
        HideMenu ();
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.Escape) && !InventoryUI.shown) {
            escapeMenu.gameObject.SetActive (!escapeMenu.gameObject.activeInHierarchy);
        }
    }

    private void HideMenu () {
        escapeMenu.gameObject.SetActive (false);
    }

    private void ShowMenu () {
        escapeMenu.gameObject.SetActive (true);
    }

    public void Back () {
        HideMenu ();
    }

    public void Quit () {
        GameManager.Quit();
    }

    public void Logout () {
        if (Client.connected)
            Client.Logout ();
        else
            GameManager.Quit ();
    }
}
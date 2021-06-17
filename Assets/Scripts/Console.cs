using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Console : MonoBehaviour {
    public static bool shown { get; private set; }
    static Console instance;
    static List<string> queuedMessages = new List<string> ();
    static List<string> previousMessages = new List<string> ();

    [SerializeField] GameObject content;
    [SerializeField] RectTransform messagesParent;
    [SerializeField] TMP_Text textPrefab;

    public static void Clear () {
        foreach (Transform transform in instance.messagesParent) {
            Destroy (transform);
        }
    }

    public static void Show () {
        instance.content.SetActive (true);
        shown = true;
    }

    public static void Hide () {
        instance.content.SetActive (false);
        shown = false;
    }

    public static void Log (object message, string colour = "white") {
        // black, blue, green, orange, purple, red, white, and yellow.
        lock (queuedMessages) {
            queuedMessages.Add ($"<color={colour}>{message}</color>");
        }
        Debug.Log (message);
    }

    public static void LogError (object message) {
        lock (queuedMessages) {
            queuedMessages.Add ($"\n<color=red>{message}</color>");
        }
        Debug.LogError (message);
    }

    static void AddMessage (string message) {
        TMP_Text newText = Instantiate (instance.textPrefab, Vector3.zero, Quaternion.identity, instance.messagesParent);
        newText.text = message;
    }

    void Awake () {
        if (instance != null) {
            Destroy (gameObject);
            return;
        } else {

        }

        instance = this;
        if (shown) Show ();
        else Hide ();
    }
    void Start () {
        foreach (string message in previousMessages) {
            AddMessage (message);
        }
    }

    void Update () {
        if (shown) {
            if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.F1)) {
                Hide ();
            }
        } else if (Input.GetKeyDown (KeyCode.F1)) {
            Show ();
        }
    }

    void FixedUpdate () {
        if (queuedMessages.Count > 0) {
            string[] newMessages;
            lock (queuedMessages) {
                newMessages = queuedMessages.ToArray ();
                queuedMessages.Clear ();
            }
            foreach (string message in newMessages) {
                AddMessage (message);
                previousMessages.Add (message);
            }
        }
    }
}
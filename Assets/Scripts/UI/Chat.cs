using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour {
    public static bool typing { get; private set; }
    static Chat instance;

    [SerializeField] RectTransform messages;
    [SerializeField] TMP_Text messagePrefab;
    [SerializeField] TMP_InputField inputField;

    public static void AddMessage (string message) {
        TMP_Text newMessage = Instantiate (instance.messagePrefab, Vector3.zero, Quaternion.identity, instance.messages);
        newMessage.text = message;
    }

    public static void Focus () {
        instance.inputField.ActivateInputField ();
        typing = true;
    }

    public static void Unfocus () {
        instance.inputField.DeactivateInputField ();
        instance.inputField.text = "";
        typing = false;
    }

    public static void SendMessage () {
        string message = instance.inputField.text.Trim();
        if (message == "") return;
        instance.inputField.text = "";

        AddMessage (message);

        PacketSender.ChatMessage (message);
    }

    void Awake () {
        instance = this;
    }

    void Start () {
        Unfocus ();

        inputField.onSelect.AddListener ((string _) => {
            Focus ();
        });
        inputField.onDeselect.AddListener ((string _) => {
            Unfocus ();
        });
    }

    void Update () {
        if (typing) {
            if (Input.GetKeyDown (KeyCode.Escape)) {
                Unfocus ();
            } else if (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return)) {
                SendMessage ();
            }
        } else if (!GameManager.showingUI) {
            if (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return))
                Focus ();
        }
    }
}
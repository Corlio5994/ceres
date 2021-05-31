using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour {
    private static Chat instance;
    public static bool typing { get; private set; }

    [SerializeField] private RectTransform messages;
    [SerializeField] private TMP_Text messagePrefab;
    [SerializeField] private TMP_InputField inputField;

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
}
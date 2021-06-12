using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This class enables creation of chat windows where users can input and send messages
public class Chat : MonoBehaviour {
    //uses an instance of Chat class, a bool to indicate if user is typing
    private static Chat instance;
    public static bool typing { get; private set; }
    //RectTransform, TMP_TEXT and InputField appear in Unity editor
    //TMP_Text appears on RectTransform, and inputField is the area users
    //type into
    [SerializeField] private RectTransform messages;
    [SerializeField] private TMP_Text messagePrefab;
    [SerializeField] private TMP_InputField inputField;

    //initialising method
    void Awake () {
        instance = this;
    }

    //initialise chat by unfocusing (to ensure user isn't automatically typing)
    //if the inputfield is selected with the mouse, a listener is added to the focus method, and the focus method is called
    //if inputField is deselected, unfocus is called
    //delegate used to enable addListener to treat void method as
    void Start () {
        Unfocus ();

        inputField.onSelect.AddListener(delegate { Focus(); });
        inputField.onDeselect.AddListener (delegate { Unfocus(); });
    }

    //method for updating state: 
    //if user is typing and holding the escape key, input is deactivated. 
    //if user is typing and pressing enter or return, message is sent
    //if the user is not typing and the UI is not visible,
    //the input box is focussed on if enter or return is pressed
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

    //takes a string message as input parameter, and creates a new 
    //TMP_Text instance containing this message
    //text is not rotated and placed using the 0 vector in 3d-space (this can be varied if text shadow is desired)
    public static void AddMessage (string message) {
        TMP_Text newMessage = Instantiate (instance.messagePrefab, Vector3.zero, Quaternion.identity, instance.messages);
        newMessage.text = message;
    }

    //sets typing to true, and activates an input field using the TMP_Text method
    public static void Focus () {
        instance.inputField.ActivateInputField ();
        typing = true;
    }

    //deactivates input field, clears input field text and sets typing to false
    public static void Unfocus () {
        instance.inputField.DeactivateInputField ();
        instance.inputField.text = "";
        typing = false;
    }

    //converts the inputField text into a string with whitespace removed
    //doesn't send message if an empty message was submitted
    //adds message, and sends the TCP packet using the chatmessage method
    public static void SendMessage () {
        string message = instance.inputField.text.Trim();
        if (message == "") return;
        instance.inputField.text = "";

        AddMessage (message);
        PacketSender.ChatMessage (message);
    }
}
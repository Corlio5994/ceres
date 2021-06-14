using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContainerUI : MonoBehaviour {
    public static bool shown { get; private set; }
    public static Container container { get; private set; }
    private static ContainerUI instance;
    private static TMP_Text currentItemText;
    private static Item selectedItem;
    private static bool deposit;
    private static Dictionary<int, TMP_Text> playerItemsDisplays;
    private static Dictionary<int, TMP_Text> containerItemsDisplays;

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject layout;
    [SerializeField] private GameObject withdrawButton;
    [SerializeField] private GameObject depositButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private RectTransform playerItemsParent;
    [SerializeField] private RectTransform containerItemsParent;
    [SerializeField] private TMP_Text itemPrefab;

    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemCategory;
    [SerializeField] private TMP_Text itemWeight;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemDescription;

    [SerializeField] private Color textColour;
    [SerializeField] private Color selectedColour;

    void Awake () {
        instance = this;
    }

    void Start () {
        Hide ();
    }

    void Update () {
        if ((Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Tab)) && shown) {
            container.Close ();
            Hide ();
        }
    }

    public static void Show (Container container, bool reset = true) {
        ContainerUI.container = container;

        playerItemsDisplays = new Dictionary<int, TMP_Text> ();
        containerItemsDisplays = new Dictionary<int, TMP_Text> ();

        foreach (Transform child in instance.playerItemsParent) {
            Destroy (child.gameObject);
        }
        foreach (Transform child in instance.containerItemsParent) {
            Destroy (child.gameObject);
        }
        if (reset)
            currentItemText = null;

        List<Item> playerItems = Player.instance.inventory.GetSortedItems ();
        foreach (Item item in playerItems) {
            CreateItemButton (item, instance.playerItemsParent, true, playerItemsDisplays);
        }
        List<Item> containerItems = container.inventory.GetSortedItems ();
        foreach (Item item in containerItems) {
            CreateItemButton (item, instance.containerItemsParent, false, containerItemsDisplays);
        }

        if (reset)
            ShowAnyItem ();
        else
            ShowItem (selectedItem, deposit);

        shown = true;
        instance.content.SetActive (true);
    }

    public static void CreateItemButton (Item item, Transform parent, bool deposit, Dictionary<int, TMP_Text> data) {
        TMP_Text text = Instantiate (instance.itemPrefab, Vector3.zero, Quaternion.identity, parent);


        text.text = $"{item.count}x {item.name}";
        for (int i = 0; i < parent.childCount; i++) {
            TMP_Text otherText = parent.GetChild(i).GetComponent<TMP_Text>();
            if (string.Compare(text.text, otherText.text) < 0) {
                text.transform.SetSiblingIndex(i);
            }
        }

        Button button = text.GetComponent<Button> ();
        UnityAction listener = () => {
            ShowItem (item, deposit);
        };

        data.Add (item.id, text);

        button.onClick.AddListener (listener);
        if (currentItemText == null) listener.Invoke ();
    }

    public static void ShowAnyItem () {
        if (Player.instance.inventory.itemCount > 0) {
            ShowItem (Player.instance.inventory.GetSortedItems () [0], true);
        } else if (container.inventory.itemCount > 0) {
            ShowItem (container.inventory.GetSortedItems () [0], false);
        } else {
            instance.layout.SetActive (false);
        }
    }

    public static void ShowItem (Item item, bool deposit) {
        selectedItem = item;
        ContainerUI.deposit = deposit;

        instance.itemTitle.text = item.name;
        instance.itemCategory.text = item.category;
        instance.itemWeight.text = $"{item.weight} kg";
        instance.itemCount.text = $"{item.count}x";
        instance.itemDescription.text = item.description;

        if (currentItemText != null) {
            currentItemText.color = instance.textColour;
        }

        if (deposit) currentItemText = playerItemsDisplays[item.id];
        else currentItemText = containerItemsDisplays[item.id];

        currentItemText.color = instance.selectedColour;

        instance.depositButton.SetActive (deposit);
        instance.withdrawButton.SetActive (!deposit);

        instance.layout.SetActive (true);
    }

    public static void RemoveItemButton (int id, Dictionary<int, TMP_Text> data) {
        Destroy (data[id].gameObject);
        data.Remove (id);
    }

    public static void Hide () {
        shown = false;
        instance.content.SetActive (false);
    }

    public void Deposit () {
        if (selectedItem == null) return;

        int id = selectedItem.id;
        Item deposited = ItemDatabase.GetItem (id);
        int leftovers = Player.instance.RemoveItem (id);
        instance.itemCount.text = $"{selectedItem.count}x";

        if (leftovers <= 0) {
            RemoveItemButton (id, playerItemsDisplays);
            ShowAnyItem ();
        } else {
            currentItemText.text = $"{selectedItem.count}x {selectedItem.name}";
        }

        if (containerItemsDisplays.ContainsKey (id)) {
            container.inventory.AddItem (deposited);
            Item newAmount = container.inventory.GetItem (id);
            containerItemsDisplays[id].text = $"{newAmount.count}x {newAmount.name}";
        } else {
            container.inventory.AddItem (deposited);
            CreateItemButton (deposited, instance.containerItemsParent, false, containerItemsDisplays);
        }

        PacketSender.BankDeposit (container, deposited);
    }

    public void Withdraw () {
        if (selectedItem == null) return;

        int id = selectedItem.id;
        Item withdrawn = ItemDatabase.GetItem (id);
        if (!Player.instance.inventory.HasSpace (withdrawn)) return;

        int leftovers = container.inventory.RemoveItem (id);
        instance.itemCount.text = $"{selectedItem.count}x";

        if (leftovers <= 0) {
            RemoveItemButton (id, containerItemsDisplays);
            ShowAnyItem ();
        } else {
            currentItemText.text = $"{selectedItem.count}x {selectedItem.name}";
        }

        if (playerItemsDisplays.ContainsKey (id)) {
            Player.instance.AddItem (withdrawn);
            Item newAmount = Player.instance.inventory.GetItem (id);
            playerItemsDisplays[id].text = $"{newAmount.count}x {newAmount.name}";
        } else {
            Player.instance.AddItem (withdrawn);
            CreateItemButton (withdrawn, instance.playerItemsParent, true, playerItemsDisplays);
        }

        PacketSender.BankWithdraw (container, withdrawn);
    }
}
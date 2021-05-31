using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour {
    public static bool shown { get; private set; } = false;
    private static InventoryUI instance;
    private static Item selectedItem;
    private static TMP_Text currentItemText;
    private static Dictionary<int, TMP_Text> itemDisplays;

    [SerializeField] private TMP_Text itemPrefab;

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject layout;
    [SerializeField] private Transform itemsList;

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
        if ((Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && shown) {
            Hide ();
        }

        if (Input.GetKeyDown (KeyCode.Tab) && !GameManager.showingUI) {
            Show ();
        }
    }

    public static void Show () {
        itemDisplays = new Dictionary<int, TMP_Text> ();

        foreach (Transform child in instance.itemsList) {
            Destroy (child.gameObject);
        }
        currentItemText = null;

        List<Item> playerItems = Player.instance.inventory.GetSortedItems ();
        foreach (Item item in playerItems) {
            CreateItemButton (item);
        }

        ShowAnyItem ();

        shown = true;
        instance.content.SetActive (true);
    }

    public static void CreateItemButton (Item item) {
        TMP_Text text = Instantiate (instance.itemPrefab, Vector3.zero, Quaternion.identity, instance.itemsList);
        Button button = text.GetComponent<Button> ();

        text.text = $"{item.count}x {item.name}";

        UnityAction listener = () => {
            ShowItem (item);
        };

        itemDisplays.Add (item.id, text);

        button.onClick.AddListener (listener);
        if (currentItemText == null) listener.Invoke ();
    }

    public static void ShowAnyItem () {
        if (Player.instance.inventory.itemCount > 0) {
            ShowItem (Player.instance.inventory.GetSortedItems () [0]);
        } else {
            instance.layout.SetActive (false);
        }
    }

    public static void ShowItem (Item item) {
        selectedItem = item;

        instance.itemTitle.text = item.name;
        instance.itemCategory.text = item.category;
        instance.itemWeight.text = $"{item.weight} kg";
        instance.itemCount.text = $"{item.count}x";
        instance.itemDescription.text = item.description;

        if (currentItemText != null) {
            currentItemText.color = instance.textColour;
        }

        currentItemText = itemDisplays[item.id];

        currentItemText.color = instance.selectedColour;

        instance.layout.SetActive (true);
    }

    public static void RemoveItemButton (int id) {
        Destroy (itemDisplays[id].gameObject);
        itemDisplays.Remove (id);
    }

    public static void Hide () {
        shown = false;
        instance.content.SetActive (false);
    }

    public void Drop () {
        if (selectedItem == null) return;

        int id = selectedItem.id;
        int leftovers = Player.instance.DropItem (id);
        instance.itemCount.text = $"{selectedItem.count}x";

        if (leftovers <= 0) {
            RemoveItemButton (id);
            ShowAnyItem ();
        } else {
            currentItemText.text = $"{selectedItem.count}x {selectedItem.name}";
        }

        
    }
}
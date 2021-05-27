using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    public static bool shown { get; private set; } = false;
    private static InventoryUI instance;
    private static Item selectedItem;
    private static TMP_Text currentItemText;

    [SerializeField] private TMP_Text itemPrefab;

    [SerializeField] private GameObject inventoryParent;
    [SerializeField] private GameObject content;
    [SerializeField] private Transform itemsList;

    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemCategory;
    [SerializeField] private TMP_Text itemWeight;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemDescription;

    void Awake () {
        instance = this;
    }

    void Start () {
        Hide ();
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.Escape) && shown) {
            Hide ();
        }

        if (Input.GetKeyDown (KeyCode.Tab)) {
            if (shown)
                Hide ();
            else
                Show ();
        }
    }

    public static void Show () {
        shown = true;

        foreach (Transform child in instance.itemsList) {
            Destroy (child.gameObject);
        }

        List<Item> items = Player.instance.inventory.GetSortedItems ();
        foreach (Item item in items) {
            TMP_Text newItem = Instantiate (instance.itemPrefab, Vector3.zero, Quaternion.identity, instance.itemsList);
            newItem.text = $"{item.count}x {item.name}";
            newItem.GetComponent<Button> ().onClick.AddListener (() => {
                ShowItem (item);
                currentItemText = newItem;
            });
            if (currentItemText == null) currentItemText = newItem;
        }

        if (items.Count > 0)
            ShowItem (items[0]);
        else
            instance.content.SetActive (false);

        instance.inventoryParent.SetActive (true);
    }

    public static void ShowItem (Item item) {
        selectedItem = item;

        instance.itemTitle.text = item.name;
        instance.itemCategory.text = item.category;
        instance.itemWeight.text = $"{item.weight} kg";
        instance.itemCount.text = $"{item.count}x";
        instance.itemDescription.text = item.description;

        instance.content.SetActive (true);
    }

    public static void Hide () {
        shown = false;

        instance.inventoryParent.SetActive (false);
    }

    public void Drop () {
        if (selectedItem == null) return;

        int leftover = Player.instance.DropItem (selectedItem.id);
        if (leftover <= 0) {
            Show ();
        } else {
            currentItemText.text = $"{selectedItem.count}x {selectedItem.name}";
            itemCount.text = $"{selectedItem.count}x";
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    private static ItemDatabase instance;
    private static Items categorisedItems;
    private static Dictionary<int, Item> items = new Dictionary<int, Item> ();
    public static Color[] rarityColours;

    [SerializeField] public Color[] _rarityColours;
    [SerializeField] private string textAssetPath;
    [SerializeField] private ItemPickup itemPickupPrefab;

    void Awake () {
        instance = this;
        TextAsset jsonFile = Resources.Load<TextAsset> (textAssetPath);
        categorisedItems = JsonUtility.FromJson<Items> (jsonFile.text);
        rarityColours = _rarityColours;

        foreach (var item in categorisedItems.weapons)
            items.Add (item.id, item);
        foreach (var item in categorisedItems.armours)
            items.Add (item.id, item);
        foreach (var item in categorisedItems.food)
            items.Add (item.id, item);
        foreach (var item in categorisedItems.potions)
            items.Add (item.id, item);
        foreach (var item in categorisedItems.materials)
            items.Add (item.id, item);
        foreach (var item in categorisedItems.miscelaneous)
            items.Add (item.id, item);
    }

    public static Item GetItem (int id, int count = 1) {
        Item item = (Item) items[id].Clone ();
        item.count = count;
        return item;
    }

    public static void SpawnItemPickup (int id, int count, Vector3 position) {
        ItemPickup pickup = Instantiate (instance.itemPickupPrefab, position, Quaternion.identity);
        Item item = GetItem (id, count);
        pickup.SetItem (item);
    }

    [System.Serializable]
    private class Items {
        public Weapon[] weapons;
        public Armour[] armours;
        public Food[] food;
        public Potion[] potions;
        public Material[] materials;
        public Item[] miscelaneous;
    }
}
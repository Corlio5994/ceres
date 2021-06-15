using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static Color[] rarityColours;
    static ItemDatabase instance;
    static Items categorisedItems;
    static Dictionary<int, Item> items = new Dictionary<int, Item> ();

    [SerializeField] public Color[] _rarityColours;
    [SerializeField] string textAssetPath;
    [SerializeField] ItemPickup itemPickupPrefab;

    public static Item GetItem (int id, int count = 1) {
        Item item = (Item) items[id].Clone ();
        item.count = count;
        return item;
    }

    public static ItemPickup SpawnItemPickup (int id, int count, Vector3 position, int pickupID = -1) {
        ItemPickup pickup = Instantiate (instance.itemPickupPrefab, position, Quaternion.identity);
        Item item = GetItem (id, count);
        pickup.SetItem (item, pickupID);
        return pickup;
    }

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

    [System.Serializable]
    class Items {
        public Weapon[] weapons;
        public Armour[] armours;
        public Food[] food;
        public Potion[] potions;
        public Material[] materials;
        public Item[] miscelaneous;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    [SerializeField] private Color[] rarityColours;
    [SerializeField] private string textAssetPath;
    private static Items categorisedItems;
    private static Dictionary<int, Item> items = new Dictionary<int, Item> ();

    void Start () {
        TextAsset jsonFile = Resources.Load<TextAsset> (textAssetPath);
        categorisedItems = JsonUtility.FromJson<Items> (jsonFile.text);

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

    public static Item GetItem (int id) {
        return items[id];
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
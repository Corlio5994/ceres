using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory {
    [SerializeField] private List<Item> items = new List<Item> ();
    public readonly float maxWeight;
    [SerializeField] private float weight = 0;
    public int itemCount { get { return items.Count; } }

    private float availableSpace { get { return maxWeight - weight; } }

    public Inventory(float maxWeight = 30) {
        this.maxWeight = maxWeight;
    }

    public Item AddItem (Item item) {
        if (availableSpace < item.weight) return item;

        if (HasSpace (item)) {
            Item existingItem = GetItem (item.id);
            if (existingItem == null) {
                items.Add (item);
            } else {
                existingItem.count += item.count;
            }
            weight += item.count * item.weight;
            return null;
        } else {
            Item leftovers = (Item) item.Clone ();
            item.count = Mathf.FloorToInt (availableSpace / item.weight);
            leftovers.count -= item.count;

            AddItem (item);
            return leftovers;
        }
    }

    public void SetItems (List<Item> items) {
        this.items = items;
    }

    public List<Item> GetSortedItems () {
        return items.OrderBy (item => item.name).ToList ();
    }

    public T GetItem<T> (int id) where T : Item {
        return (T) GetItem (id);
    }

    public Item GetItem (int id) {
        foreach (Item item in items) {
            if (item.id == id)
                return item;
        }
        return null;
    }

    public int RemoveItem (int id, int count = 1) {
        Item item = GetItem (id);
        if (item == null) return 0;

        if (item.count > count) {
            item.count -= count;
            weight -= count * item.weight;
            return item.count;
        } else {
            items.Remove (item);
            weight -= item.count * item.weight;
            return 0;
        }

    }

    public bool HasSpace (Item item) {
        return weight + item.weight * item.count <= maxWeight;
    }
}
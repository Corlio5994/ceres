using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable {
    public static readonly Dictionary<int, Container> containers = new Dictionary<int, Container> ();

    [HideInInspector] public Inventory inventory { get; private set; } = new Inventory (9999);
    [SerializeField] Animator animator;
    [HideInInspector] public int id { get; private set; } = -1;

    void Start () {
        containers.Add (id, this);

        inventory.AddItem (ItemDatabase.GetItem (0, 10));
        inventory.AddItem (ItemDatabase.GetItem (1, 10));
    }

    public static Container Get (int containerID) {
        if (!containers.ContainsKey(containerID)) return null;
        return containers[containerID];
    }

    public override void Interact (Entity entity) {
        ContainerUI.Show (this);
        Open ();
    }

    public void Open () {
        animator.SetTrigger ("open");
    }

    public void Close () {
        animator.SetTrigger ("close");
    }

    protected override void OnMouseOver () {
        base.OnMouseOver ();
        TooltipUI.ShowTooltip (name);
    }

    public List<Item> GetItems () {
        return inventory.GetSortedItems ();
    }

    public Item Deposit (Item item) {
        return inventory.AddItem (item);
    }

    public Item Withdraw (int id, int count = 1) {
        Item item = inventory.GetItem (id);
        int takenCount = Mathf.Min (item.count, count);
        item.count -= takenCount;
        Item takenItem = (Item) item.Clone ();
        takenItem.count = takenCount;
        return takenItem;
    }
}
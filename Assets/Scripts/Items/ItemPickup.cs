using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable {
    [SerializeField] private int itemID;
    [SerializeField] private int count = 1;
    private Item item;

    void Start () {
        item = ItemDatabase.GetItem (itemID, count);
        name = item.name;
    }

    public void SetItem (Item item) {
        this.item = item;
    }

    public override void Interact (Entity entity) {
        item = entity.AddItem (item);
        if (item == null) Destroy (gameObject);
    }

    protected override void OnMouseOver () {
        base.OnMouseOver ();
        TooltipUI.ShowTooltip ($"{item.count}x {item.name}", item.category, item.description);
        TooltipUI.SetTooltipRarity (ItemDatabase.rarityColours[item.rarity]);
    }
}
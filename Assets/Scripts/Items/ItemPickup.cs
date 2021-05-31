using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable {
    private Item item;
    private static int itemDespawnTime = 300;

    void Start () {
        Destroy (gameObject, itemDespawnTime);
    }

    void CheckSurroundingPickups () {
        Collider[] hitColliders = Physics.OverlapSphere (transform.position, interactRadius, GameManager.interactableMask);
        foreach (var hitCollider in hitColliders) {
            ItemPickup pickup = hitCollider.GetComponent<ItemPickup> ();
            if (pickup != null && pickup != this && pickup.item.id == item.id) {
                pickup.item.count += item.count;
                Destroy (gameObject);
                return;
            }
        }
    }

    public void SetItem (Item item) {
        this.item = item;
        CheckSurroundingPickups ();
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
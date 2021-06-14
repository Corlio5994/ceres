using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Entity : MonoBehaviour {
    private NavMeshAgent agent;
    private Interactable interactable;

    [SerializeField] public Inventory inventory = new Inventory ();

    protected virtual void Start () {
        agent = GetComponent<NavMeshAgent> ();
    }

    void FixedUpdate () {
        if (interactable != null && interactable.InRange (transform.position)) {
            interactable.Interact (this);
            interactable = null;
            Stop ();
        }
    }

    protected virtual void Update () {
        
    }

    public Item AddItem (Item item) {
        return inventory.AddItem (item);
    }

    public int RemoveItem (int id, int count = 1) {
        return inventory.RemoveItem (id, count);
    }

    public int DropItem (int id, int count = 1) {
        int leftoverCount = RemoveItem (id, count);
        ItemPickup pickup = ItemDatabase.SpawnItemPickup (id, count, transform.position);
        PacketSender.ItemDropped(pickup);
        return leftoverCount;
    }

    public void SetDestination (Vector3 destination) {
        agent.SetDestination (destination);
    }

    public virtual void Stop () {
        SetDestination (transform.position);
    }

    protected void Interact (Interactable interactable) {
        this.interactable = interactable;
        SetDestination (interactable.transform.position);
    }
}
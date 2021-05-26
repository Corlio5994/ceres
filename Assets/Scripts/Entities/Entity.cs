using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Entity : MonoBehaviour {
    private NavMeshAgent agent;
    private Interactable interactable;

    protected virtual void Start () {
        agent = GetComponent<NavMeshAgent> ();
    }

    void FixedUpdate () {
        if (interactable != null && interactable.InRange (transform.position)) {
            interactable.Interact (this);
            Stop();
        }
    }

    public void SetDestination (Vector3 destination) {
        agent.SetDestination (destination);
    }

    public void Stop() {
        SetDestination(transform.position);
    }

    protected void Interact (Interactable interactable) {
        this.interactable = interactable;
        SetDestination (interactable.transform.position);
    }
}
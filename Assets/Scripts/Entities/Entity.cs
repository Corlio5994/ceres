using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Entity : MonoBehaviour {
    private NavMeshAgent agent;

    protected virtual void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination) {
        agent.SetDestination(destination);
    }
}
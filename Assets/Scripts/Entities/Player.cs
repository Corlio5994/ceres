using UnityEngine;

public class Player : Entity {
    public static Player instance;

    void Awake () {
        instance = this;
    }

    void Update () {
        if (GameManager.showingUI) return;

        if (Input.GetMouseButtonDown (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, Mathf.Infinity, GameManager.interactableMask)) {
                Interactable newInteractable = hit.collider.GetComponent<Interactable> ();
                Interact (newInteractable);
                PacketSender.PlayerMoved (newInteractable.transform.position);
            } else if (Physics.Raycast (ray, out hit, Mathf.Infinity, GameManager.groundMask)) {
                SetDestination (hit.point);
                PacketSender.PlayerMoved (hit.point);
            }
        }

    }

    public override void Stop () {
        base.Stop ();
        PacketSender.PlayerMoved (transform.position);
    }
}
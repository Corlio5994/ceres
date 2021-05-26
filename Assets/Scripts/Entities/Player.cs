using UnityEngine;

public class Player : Entity {
    protected virtual void Update () {
        if (EscapeMenuUI.active) return;

        if (Input.GetMouseButtonDown (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, Mathf.Infinity, GameManager.interactableMask)) {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();
                Interact(newInteractable);
                // TODO: Send to server
            }

            else if (Physics.Raycast (ray, out hit, Mathf.Infinity, GameManager.groundMask)) {
                SetDestination (hit.point);
                PacketSender.PlayerMoved (hit.point);
            }
        }
    }
}
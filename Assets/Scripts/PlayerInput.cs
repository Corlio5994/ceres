using UnityEngine;

public class PlayerInput : MonoBehaviour {
    void Update () {
        if (GameManager.showingUI || StateManager.server) return;

        if (Input.GetMouseButtonDown (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, Mathf.Infinity, GameManager.interactableMask)) {
                Interactable newInteractable = hit.collider.GetComponent<Interactable> ();
                GameManager.mainPlayer.Interact (newInteractable);
                PacketSender.PlayerMoved (newInteractable.transform.position);
            } else if (Physics.Raycast (ray, out hit, Mathf.Infinity, GameManager.groundMask)) {
                GameManager.mainPlayer.SetDestination (hit.point);
                PacketSender.PlayerMoved (hit.point);
            }
        }

    }

}
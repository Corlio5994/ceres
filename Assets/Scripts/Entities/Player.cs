using UnityEngine;

public class Player : Entity {
    protected virtual void Update() {
        if (Input.GetMouseButtonDown(0) && !EscapeMenuUI.active) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.groundMask)) {
                SetDestination(hit.point);
                PacketSender.PlayerMoved(hit.point);
            }
        }
    }
}
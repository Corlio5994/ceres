using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    protected float interactRadius = 3f;
    private bool hoveringOver = false;

    public virtual void Interact (Entity entity) {
        Destroy (gameObject);
    }

    public bool InRange (Vector3 position) {
        return Vector3.Distance (position, transform.position) < interactRadius;
    }

    public bool MouseOver () {
        return hoveringOver;
    }

    protected virtual void OnMouseOver () {
        hoveringOver = true;
    }

    void OnMouseExit () {
        hoveringOver = false;
        TooltipUI.HideTooltip();
    }

    void OnDestroy()
    {
        if (MouseOver()) {
            OnMouseExit();
        }
    }

    void OnDrawGizmosSelected () {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interactRadius);
    }
}
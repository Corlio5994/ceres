using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    static Transform target;
    static Vector3 offset;
    static float zoom;

    [SerializeField] float smoothTime = 0.5f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float minimumZoom = 5f;
    [SerializeField] float maximumZoom = 13f;
    [SerializeField] float zoomSpeed = 0.25f;

    public static void SetTarget (Transform target, bool reposition = true) {
        CameraController.target = target;
        if (reposition)
            Camera.main.transform.position = target.position + offset;
    }

    void Awake () {
        zoom = minimumZoom;
        offset = transform.position;
    }

    void Update () {
        if (target != null)
            transform.position = Vector3.Lerp (transform.position, target.transform.position + offset, smoothTime * Time.deltaTime);
        else {
            Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
            transform.position += input * moveSpeed * Time.deltaTime;
        }

        Vector2 scrollDelta = Input.mouseScrollDelta;
        zoom -= scrollDelta.y * zoomSpeed;
        if (!StateManager.server)
            zoom = Mathf.Clamp (zoom, minimumZoom, maximumZoom);
        Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, zoom, smoothTime * Time.deltaTime);
    }
}
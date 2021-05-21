using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float _smoothTime = 0.5f;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _minimumZoom = 5f;
    [SerializeField] private float _maximumZoom = 13f;
    [SerializeField] private float _zoomSpeed = 0.25f;

    private static float smoothTime;
    private static float moveSpeed;
    private static float zoom;
    private static float minimumZoom;
    private static float maximumZoom;
    private static float zoomSpeed;
    private static Transform target;
    private static Vector3 offset;
    private static Vector3 velocity;

    void Awake () {
        smoothTime = _smoothTime;
        moveSpeed = _moveSpeed;
        minimumZoom = _minimumZoom;
        maximumZoom = _maximumZoom;
        zoomSpeed = _zoomSpeed;
        zoom = minimumZoom;
        offset = transform.position;
    }

    void Update () {
        if (target != null)
            transform.position = Vector3.SmoothDamp (transform.position, target.transform.position + offset, ref velocity, smoothTime, 100f);
        else {
            Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
            transform.position += input * moveSpeed * Time.deltaTime;
        }

        Vector2 scrollDelta = Input.mouseScrollDelta;
        zoom -= scrollDelta.y * zoomSpeed;
        if (!GameServer.Server.active)
            zoom = Mathf.Clamp (zoom, minimumZoom, maximumZoom);
        Camera.main.orthographicSize = zoom;
    }

    public static void SetTarget (Transform target, bool reposition = true) {
        CameraController.target = target;
        if (reposition)
            Camera.main.transform.position = target.position + offset;
    }
}
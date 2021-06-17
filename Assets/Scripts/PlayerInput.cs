using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private Rigidbody rigidBody;
    public float moveSpeed;
    private Vector3 targetRotation;
    public float smoothTime;
    public static Vector2 input;
    void Update () {
        if (GameManager.showingUI || StateManager.server) return;
        if (rigidBody == null){
            rigidBody = GameManager.mainPlayer.GetComponent<Rigidbody>();
        }
        float horzInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");
        rigidBody.velocity = new Vector3(horzInput,0,vertInput)*moveSpeed*Time.deltaTime;
        if (horzInput != 0 || vertInput != 0)
        {
            targetRotation = new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(horzInput,vertInput), 0);
        }
        if (targetRotation.y >= 180)
        {
            targetRotation.y -= 360;
        }
        else if (targetRotation.y <= -180)
            {
            targetRotation.y += 360;
            }
        rigidBody.transform.rotation = Quaternion.Lerp(rigidBody.transform.rotation, Quaternion.Euler(targetRotation), smoothTime * Time.deltaTime);
        input = new Vector2(horzInput, vertInput);
    }

}
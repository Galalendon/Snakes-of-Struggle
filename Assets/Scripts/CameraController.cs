using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The snake's transform to follow
    public Vector3 offset;   // Offset from the target position

    public float smoothSpeed = 5.0f; // Adjust this to control camera smoothing   
    public float cameraMoveSpeed = 5f;
    public float cameraSnapSpeed = 10f;
    public float deadzone = 0.1f;
    public Vector3 maxOffset;

    public string horizontal;
    public string vertical;

    private Vector3 moveOffset;
    public bool flip = false;

    private void Start()
    {
        string player2LayerName = "Player2";
        int player2Layer = LayerMask.NameToLayer(player2LayerName);

        if (target.gameObject.layer == player2Layer)
        {
            flip = true;
        }
    }

    private void Update()
    {
        Vector2 input;

        if (flip)
        {
            input = new Vector2(-Input.GetAxis(horizontal), -Input.GetAxis(vertical)); // Invert both horizontal and vertical input
        }
        else
        {
            input = new Vector2(Input.GetAxis(horizontal), Input.GetAxis(vertical));
        }
        
        if (input.sqrMagnitude > deadzone * deadzone)
        {
            // Move camera based on input, capped by max offset
            moveOffset += new Vector3(input.x, input.y, 0f) * cameraMoveSpeed * Time.deltaTime;
            moveOffset = Vector3.ClampMagnitude(moveOffset, maxOffset.magnitude);
        }
        else
        {
            // Gradually move camera back to centered position
            moveOffset = Vector3.Lerp(moveOffset, Vector3.zero, cameraSnapSpeed * Time.deltaTime);
        }

        // Calculate the desired camera position
        Vector3 desiredPosition = target.position + offset + moveOffset;

        // Use Lerp to smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
   
}

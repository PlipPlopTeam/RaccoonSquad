using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Referencies
    public PlayerInputs controller;
    Rigidbody rb;

    [Header("Locomotion")]
    public float speedForce = 25f;
    public float orientationLerpSpeed = 10f;

    Vector3 targetOrientation;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Debug
        if(controller.gamepad.GetButtonDown("A")) Debug.Log(gameObject.name);

        Movement();
    }

    void Movement()
    {
        // Calculate the direction of the movement depending on the gamepad inputs
        Vector2 direction = new Vector3(controller.gamepad.GetStick_L().X, controller.gamepad.GetStick_L().Y);
        rb.AddForce(new Vector3(direction.x, 0f, direction.y) * speedForce * Time.deltaTime);

        // If the player movement axis are still flowing
        if(direction.x != 0 || direction.y != 0) 
        {
            targetOrientation = new Vector3(direction.x, 0f, direction.y);
        }

        // Rotate the character towards his movement direction
        transform.forward = Vector3.Lerp(transform.forward, targetOrientation, Time.deltaTime * orientationLerpSpeed);
    }
}

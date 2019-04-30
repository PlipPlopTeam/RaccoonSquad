using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{

    // Referencies
    public PlayerIndex index;
    Rigidbody rb;

    [Header("Locomotion")]
    public float speedForce = 25f;
    public float orientationLerpSpeed = 10f;

    [Header("Grabbotion")]
    public List<Grabbable> objectsAtRange = new List<Grabbable>();

    CollisionEventTransmitter grabCollisions;
    Vector3 targetOrientation;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        /*
        // Check which objects are currently grabbable
        grabCollisions = GetComponentInChildren<CollisionEventTransmitter>();
        grabCollisions.onTriggerEnter += (Collider x) => { var grab = x.GetComponent<Grabbable>(); if (grab) objectsAtRange.Add(grab); };
        grabCollisions.onTriggerExit += (Collider x) => { var grab = x.GetComponent<Grabbable>(); if (grab) objectsAtRange.Remove(grab); };
        */
    }

    void Update()
    {
        // Debug
        //if(controller.gamepad.GetButtonDown("A")) Debug.Log(gameObject.name);
        var state = GamePad.GetState(index);
        CheckInputs(state);
    }

    void CheckInputs(GamePadState state)
    {
        CheckMovementInputs(state);
        CheckGrabInputs(state);
    }

    void CheckMovementInputs(GamePadState state)
    {
        // Calculate the direction of the movement depending on the gamepad inputs
        Vector2 direction = new Vector3(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        rb.AddForce(new Vector3(direction.x, 0f, direction.y) * speedForce * Time.deltaTime);

        // If the player movement axis are still flowing
        if(direction.x != 0 || direction.y != 0) 
        {
            targetOrientation = new Vector3(direction.x, 0f, direction.y);
        }

        // Rotate the character towards his movement direction
        transform.forward = Vector3.Lerp(transform.forward, targetOrientation, Time.deltaTime * orientationLerpSpeed);
    }

    void CheckGrabInputs(GamePadState state)
    {
        
    }

    void GrabProp(Grabbable prop)
    {

    }
}

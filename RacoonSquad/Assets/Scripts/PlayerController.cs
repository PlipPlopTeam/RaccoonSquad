using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    public PlayerIndex index;

    [Header("Inputs")]
    public float grabTriggerThreshold = 0.3f;

    [Header("Locomotion")]
    public float speedForce = 25f;
    public float orientationLerpSpeed = 10f;

    [Header("Grabbotion")]
    public List<Grabbable> objectsAtRange = new List<Grabbable>();

    CollisionEventTransmitter grabCollisions;
    Grabbable heldObject;
    Vector3 targetOrientation;
    Rigidbody rb;
    new CapsuleCollider collider;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        // Check which objects are currently grabbable
        grabCollisions = GetComponentInChildren<CollisionEventTransmitter>();
        grabCollisions.onTriggerEnter += (Collider x) => { var grab = x.GetComponent<Grabbable>(); if (grab) objectsAtRange.Add(grab); };
        grabCollisions.onTriggerExit += (Collider x) => { var grab = x.GetComponent<Grabbable>(); if (grab) objectsAtRange.Remove(grab); };
        
    }

    void Update()
    {
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
        if (state.Triggers.Right > grabTriggerThreshold) {
            // Trigger is pressed
            if (IsHolding()) {
                // Nothing - keep holding
            }
            else if (IsAnythingAtRange()) {
                // Grab the first object you saw
                GrabProp(objectsAtRange[0]);
            }
        }
        else {
            if (IsHolding()) {
                DropHeldObject();
            }
        }
    }

    void GrabProp(Grabbable prop)
    {
        float headHeight = 
            collider.bounds.extents.y / 2f 
            + prop.collider.bounds.extents.y / 2f 
            - prop.collider.bounds.center.y 
            + collider.bounds.center.y;

        prop.GetHeldBy(transform, new Vector3(0f, headHeight, 0f));
        heldObject = prop;
    }

    void DropHeldObject()
    {
        heldObject.GetDropped();
        heldObject = null;
    }

    void ThrowHeldObject()
    {

    }

    bool IsHolding()
    {
        return heldObject != null;
    }

    bool IsAnythingAtRange()
    {
        return objectsAtRange.Count > 0;
    }
}

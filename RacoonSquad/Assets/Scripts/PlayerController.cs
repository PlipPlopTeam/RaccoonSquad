using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    public PlayerIndex index;

    [Header("Inputs")]
    public float grabTriggerThreshold = 0.3f;
    public MeshRenderer noseRenderer;

    [Header("Locomotion")]
    public float speedForce = 25f;
    public float orientationLerpSpeed = 10f;

    [Header("Grabbotion")]
    public List<Grabbable> objectsAtRange = new List<Grabbable>();
    public float throwForceMultiplier = 100f;
    public float throwVerticality = 0.2f;

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

    void Start()
    {
        LoadNoseColor();
    }

    void LoadNoseColor()
    {
        noseRenderer.material = Instantiate(noseRenderer.material);
        switch(index)
        {
            case PlayerIndex.One:
                noseRenderer.material.color = Color.blue;
                break;
            case PlayerIndex.Two:
                noseRenderer.material.color = Color.red;
                break;
            case PlayerIndex.Three:
                noseRenderer.material.color = Color.green;
                break;
            case PlayerIndex.Four:
                noseRenderer.material.color = Color.yellow;
                break;
        }
    } // Change the color of the nose of the capsule to differentiate players (debug purpose)


    public Grabbable GetHeldObject()
    {
        return heldObject;
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

        // If the player is aiming
        if(state.ThumbSticks.Right.X != 0 || state.ThumbSticks.Right.Y != 0)
        {
            targetOrientation = new Vector3(state.ThumbSticks.Right.X, 0f, state.ThumbSticks.Right.Y);
        }
        else if(direction.x != 0 || direction.y != 0) // If the player movement axis are still flowing
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
                GrabObject(objectsAtRange[0]);
            }
        }
        else {
            if (IsHolding()) {
                var rightStickAmplitude = GetStickDirection(state.ThumbSticks.Right).magnitude;
                if (rightStickAmplitude > 0.1f) {
                    ThrowHeldObject(rightStickAmplitude*throwForceMultiplier);
                }
                else {
                    DropHeldObject();
                }
            }
        }
    }

    Vector2 GetStickDirection(GamePadThumbSticks.StickValue val) {
        return new Vector2(val.X, val.Y);
    }

    void GrabObject(Grabbable prop)
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

    void ThrowHeldObject(float force)
    {
        var prop = heldObject;
        DropHeldObject();
        prop.rigidbody.AddForce(
            Vector3.Lerp(
                transform.forward,
                transform.up,
                throwVerticality 
            )* force, ForceMode.Impulse
       );
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

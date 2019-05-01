using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    public PlayerIndex index;

    [Header("Inputs")]
    [Range(0,1)]public float grabTriggerThreshold = 0.3f;
    public MeshRenderer noseRenderer;

    [Header("Locomotion")]
    public float speedForce = 25f;
    public float orientationLerpSpeed = 10f;
    public float carryCapacity = 10f;
    [Range(0, 1)] public float minimumWeightedSpeedFactor = 0.7f;

    [Header("Grabbotion")]
    public List<Grabbable> objectsAtRange = new List<Grabbable>();
    public float throwForceMultiplier = 100f;
    [Range(0, 1)] public float throwVerticality = 0.2f;
    public float throwForceAccumulationSpeed = 1f;

    [Header("Visuals")]
    public float aimRotationSpeed = 5f;

    Sweat sweat;
    CollisionEventTransmitter grabCollisions;
    Grabbable heldObject;
    Vector3 targetOrientation;
    Rigidbody rb;
    Light aimLight;
    new CapsuleCollider collider;
    float throwAccumulatedForce = 0f;
    float throwAimForceCorrection = 0.9f; // Band-aid correction to make the spotlight aiming more accurate


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        aimLight = GetComponentInChildren<Light>();
        sweat = GetComponentInChildren<Sweat>();

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
        UpdateThrowPreview();
    }

    void CheckMovementInputs(GamePadState state)
    {
        // Calculate the direction of the movement depending on the gamepad inputs
        Vector2 direction = new Vector3(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);

        float weightModifier = 1f;
        if (IsHolding()) {
            // Slows down racoon if object carried is too heavy
            weightModifier = Mathf.Clamp(carryCapacity - heldObject.weight, 0f, 1f) * (1f - minimumWeightedSpeedFactor) + minimumWeightedSpeedFactor;
            sweat.Set(1 - weightModifier);
        }

        rb.AddForce(new Vector3(direction.x, 0f, direction.y) * speedForce * Time.deltaTime * weightModifier);

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
        bool isAccumulating = false;
        var rightStickAmplitude = GetStickDirection(state.ThumbSticks.Right).magnitude;

        if (state.Triggers.Right > grabTriggerThreshold) {
            // Trigger is pressed
            if (IsHolding()) {
                // Nothing - keep holding
                if (rightStickAmplitude > 0.1f) {
                    isAccumulating = true;
                }
            }
            else if (IsAnythingAtRange()) {
                // Grab the highest object
                GrabBestObjectAtRange();
            }
        }
        else {
            if (IsHolding()) {
                if (rightStickAmplitude > 0.1f) {
                    ThrowHeldObject(throwAccumulatedForce * throwForceMultiplier);
                }
                else {
                    DropHeldObject();
                }
            }
        }

        // Increases throw force over time, or resets it
        if (isAccumulating) {
            AccumulateThrowForce(rightStickAmplitude);
        }
        else {
            throwAccumulatedForce = 0f;
        }
    }

    void AccumulateThrowForce(float max=1f)
    {
        throwAccumulatedForce = Mathf.Clamp(
                throwAccumulatedForce + throwForceAccumulationSpeed * Time.deltaTime,
                0f,
                max
            );
    }

    void UpdateThrowPreview()
    {
        aimLight.enabled = false;
        if (throwAccumulatedForce > 0f) {
            aimLight.transform.localEulerAngles = new Vector3(90f, aimLight.transform.localEulerAngles.y + Time.deltaTime*aimRotationSpeed, 0f);
            aimLight.transform.localPosition = new Vector3(
                0f,
                aimLight.transform.localPosition.y,
                throwAccumulatedForce * throwForceMultiplier * throwAimForceCorrection
            );
            aimLight.enabled = true;
        }
    }

    void GrabBestObjectAtRange()
    {
        GrabObject(
            GetBestObjectAtRange()
        );

        sweat.Activate();
    }

    Grabbable GetBestObjectAtRange()
    {
        float bestHeight = Mathf.NegativeInfinity;
        Grabbable bestProp = null;
        foreach(var prop in objectsAtRange) {
            if (prop.transform.position.y > bestHeight) {
                bestProp = prop;
                bestHeight = prop.transform.position.y;
            }
        }
        return bestProp;
    }

    Vector2 GetStickDirection(GamePadThumbSticks.StickValue val) {
        return new Vector2(val.X, val.Y);
    }

    void GrabObject(Grabbable prop)
    {
        float headHeight = 
            collider.bounds.extents.y
            + prop.collider.bounds.extents.y 
            - prop.collider.bounds.center.y 
            + collider.bounds.center.y;

        prop.BecomeHeldBy(transform, new Vector3(0f, headHeight, 0f));
        heldObject = prop;
    }

    void DropHeldObject()
    {
        heldObject.BecomeDropped();
        heldObject = null;

        sweat.Desactivate();
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
        foreach(var prop in objectsAtRange.ToArray()) {
            if (prop == null) {
                objectsAtRange.Remove(prop);
            }
        }
        return objectsAtRange.Count > 0;
    }
}

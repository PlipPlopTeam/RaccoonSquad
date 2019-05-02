using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{

    [Header("Inputs")]
    public PlayerIndex index;
    public MeshRenderer noseRenderer;

    [Header("Locomotion")]
    public float speed = 25f;
    public float jumpForce = 100f;
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
    public float aimSpotLag = 0.05f;

    [Header("Bones")]
    public Transform rightHandBone;
    public Transform leftHandBone;

    Sweat sweat;
    FocusLook look;
    Animator anim;
    CollisionEventTransmitter grabCollisions;
    Grabbable heldObject;
    Vector3 targetOrientation;
    Rigidbody rb;
    Light aimLight;
    LineRenderer lineRenderer;
    new CapsuleCollider collider;


    float throwAccumulatedForce = 0f;
    bool acceptThrowCommands = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        look = GetComponent<FocusLook>();

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
        UpdateThrowPreview();

        if(heldObject == null)
        {
            Grabbable g = GetBestObjectAtRange();
            if(g != null) look.FocusOn(g.transform);
            else if(look.isFocused) look.LooseFocus();
        }
    }

    void CheckInputs(GamePadState state)
    {
        CheckMovementInputs(state);
        CheckGrabInputs(state);
        CheckJumpInputs(state);
    }

    void CheckJumpInputs(GamePadState state)
    {
        if(state.Buttons.A == ButtonState.Pressed && IsGrounded())
        {
            rb.AddForce(Vector3.up * Time.deltaTime * jumpForce);
            anim.SetTrigger("Jump");
        }
    }

    bool IsGrounded()
    {
        if(Physics.Raycast(transform.position, -transform.up, 0.1f)) return true;
        else return false;
    }

    void CheckMovementInputs(GamePadState state)
    {
        // Calculate the direction of the movement depending on the gamepad inputs
        Vector2 direction = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);

        float weightModifier = 1f;
        if (IsHolding()) {
            // Slows down racoon if object carried is too heavy
            weightModifier = Mathf.Clamp(carryCapacity - heldObject.weight, 0f, 1f) * (1f - minimumWeightedSpeedFactor) + minimumWeightedSpeedFactor;
            sweat.Set(1 - weightModifier);
        }

        //rb.AddForce(new Vector3(direction.x, 0f, direction.y) * speed * Time.deltaTime * weightModifier);

        transform.position += new Vector3(direction.x, 0f, direction.y) * speed * Time.deltaTime * weightModifier;
        anim.SetFloat("Speed", direction.magnitude);

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

        if (IsHolding()) {
            // Accumulate force
            if (rightStickAmplitude > 0.1f) {
                isAccumulating = true;
            }
        }

        if (state.Buttons.RightShoulder == ButtonState.Released) acceptThrowCommands = true;

        else if (state.Buttons.RightShoulder == ButtonState.Pressed && acceptThrowCommands) {
            acceptThrowCommands = false;

            // Launch is ordered
            if (IsHolding()) {
                if (rightStickAmplitude > 0.1f) {
                    ThrowHeldObject(throwAccumulatedForce * throwForceMultiplier);
                }
                else {
                    DropHeldObject();
                }
            }
            else if (IsAnythingAtRange()) {
                // Grab the highest object
                GrabBestObjectAtRange();
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
        anim.SetFloat("ThrowPercentage", throwAccumulatedForce);
    }

    void UpdateThrowPreview()
    {
        float throwAimForceCorrection = 1f - throwVerticality; // Band-aid correction to make the spotlight aiming more accurate

        aimLight.enabled = false;
        if (throwAccumulatedForce > 0f) {
            aimLight.transform.localEulerAngles = new Vector3(90f, aimLight.transform.localEulerAngles.y + Time.deltaTime*aimRotationSpeed, 0f);
            aimLight.transform.localPosition = Vector3.Lerp(
                aimLight.transform.localPosition,
                new Vector3(
                    0f,
                    aimLight.transform.localPosition.y,
                    throwAccumulatedForce * throwForceMultiplier * throwAimForceCorrection
                ),
                1f - aimSpotLag
            );
            
            aimLight.enabled = true;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(
                // Adding 0.01f to Y to avoid Z-fight
                new Vector3[] {
                    transform.position + new Vector3(0f, 0.05f, 0f),
                    Vector3.Lerp(
                        transform.position + new Vector3(0f, 0.1f, 0f),
                        new Vector3( aimLight.transform.position.x, transform.position.y+0.1f,  aimLight.transform.position.z),
                        0.92f
                    )
                }
            );
        }
        else {
            ResetThrowPreview();
        }
    }

    void ResetThrowPreview()
    {
        aimLight.transform.localPosition = new Vector3(0f, aimLight.transform.localPosition.y, 0f);
        lineRenderer.positionCount = 0;
    }

    void GrabBestObjectAtRange()
    {
        GrabObject(
            GetBestObjectAtRange()
        );
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
            collider.height/2
            + prop.GetComponent<Collider>().bounds.extents.y/2
            + collider.bounds.center.y;

        //Vector3 pos = new Vector3(prop.transform.position.x, prop.transform.position.y + headHeight, prop.transform.position.z);

        prop.BecomeHeldBy(transform, new Vector3(0f, headHeight, 0f));
        heldObject = prop;

        // Visuals
        sweat.Activate();
        anim.SetBool("Carrying", true);
    }

    void DropHeldObject()
    {
        heldObject.BecomeDropped();
        heldObject = null;

        // Visuals
        sweat.Desactivate();
        anim.SetBool("Carrying", false);
    }

    void ThrowHeldObject(float force)
    {
        var prop = heldObject;
        DropHeldObject();
        prop.GetComponent<Rigidbody>().AddForce(
            Vector3.Lerp(
                transform.forward,
                transform.up,
                throwVerticality 
            )* force, ForceMode.Impulse
        );

        throwAccumulatedForce = 0;
        anim.SetFloat("ThrowPercentage", 0);
        anim.SetTrigger("ThrowAction");
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

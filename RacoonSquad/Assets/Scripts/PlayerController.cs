using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    bool activated = true;
    public PlayerIndex index;
    public MeshRenderer noseRenderer;
    bool dead = false;

    [Header("Locomotion")]
    public float speed = 25f;
    public float jumpForce = 100f;
    public float carryCapacity = 10f;
    [Range(0, 1)] public float minimumWeightedSpeedFactor = 0.7f;
    float targetSpeed;
    float currentSpeed;

    [Header("Lerps")]
    public float speedLerpSpeed = 10f;
    public float orientationLerpSpeed = 10f;

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
    public Transform headBone;

    MovementSpeed movementSpeed;
    Sweat sweat;
    FocusLook look;
    Animator anim;
    CollisionEventTransmitter grabCollisions;
    Grabbable heldObject;
    Vector3 targetOrientation;
    Rigidbody rb;
    public GameObject aimSprite;
    LineRenderer lineRenderer;
    new CapsuleCollider collider;
    Cosmetic hat;
    Color color;

    [HideInInspector] public bool hidden;
    float throwAccumulatedForce = 0f;
    bool acceptThrowCommands = true;
    bool acceptJumps = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        look = GetComponent<FocusLook>();

        sweat = GetComponentInChildren<Sweat>();

        movementSpeed = gameObject.AddComponent<MovementSpeed>();

        // Check which objects are currently grabbable
        grabCollisions = GetComponentInChildren<CollisionEventTransmitter>();
        grabCollisions.onTriggerEnter += (Collider x) => { var grab = x.GetComponent<Grabbable>(); if (grab) objectsAtRange.Add(grab); };
        grabCollisions.onTriggerExit += (Collider x) => { var grab = x.GetComponent<Grabbable>(); if (grab) objectsAtRange.Remove(grab); };

        color = Library.instance.playersColors[(int)index];
    }

    void Update()
    {
        var state = GamePad.GetState(index);
        
        if(activated) CheckInputs(state);

        UpdateThrowPreview();
        UpdateHead();
    }

    void UpdateHead()
    {
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
        if (state.Buttons.A == ButtonState.Released) acceptJumps = true;
        if(state.Buttons.A == ButtonState.Pressed && IsGrounded() && acceptJumps)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            acceptJumps = false;
            SoundPlayer.PlayAtPosition("fb_raccoon_hop", transform.position, 0.2f, true);
        }
    }

    bool IsGrounded()
    {
        if(Physics.Raycast(transform.position, -transform.up, 0.1f)) return true;
        else return false;
    }

    void CheckMovementInputs(GamePadState state)
    {
        var rightStickAmplitude = GetStickDirection(state.ThumbSticks.Right).magnitude;
        var leftStickAmplitude = GetStickDirection(state.ThumbSticks.Left).magnitude;

        // Calculate the direction of the movement depending on the gamepad inputs
        Vector2 direction = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);

        targetSpeed = direction.magnitude * speed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedLerpSpeed);

        float weightSpeedMultiplier = 1f;
        if(IsHolding()) weightSpeedMultiplier = Mathf.Clamp(carryCapacity - heldObject.weight, 0f, 1f) * (1f - minimumWeightedSpeedFactor) + minimumWeightedSpeedFactor;
        sweat.Set(1 - weightSpeedMultiplier);

        transform.position += new Vector3(direction.x, 0f, direction.y) * currentSpeed * Time.deltaTime * movementSpeed.GetMultiplier() * weightSpeedMultiplier;
        anim.SetFloat("Speed", targetSpeed/speed);

        // If the player is aiming
        if(rightStickAmplitude > 0.1f)
        {
            targetOrientation = new Vector3(state.ThumbSticks.Right.X, 0f, state.ThumbSticks.Right.Y);
        }
        else if(leftStickAmplitude > 0.1f)
        {
            targetOrientation = new Vector3(state.ThumbSticks.Left.X, 0f, state.ThumbSticks.Left.Y);
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
                // ça ne rend pas bien, mais je crois que j'ai mis le son dans la mauvaise ligne
             //   SoundPlayer.PlayAtPosition("fb_raccoon_charging_toss", transform.position, 0.3f, true);
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




#region GRAB AND THROW

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

        aimSprite.SetActive(false);
        if (throwAccumulatedForce > 0f) {
            aimSprite.transform.localEulerAngles = new Vector3(90f, aimSprite.transform.localEulerAngles.y + Time.deltaTime*aimRotationSpeed, 0f);
            aimSprite.transform.localPosition = Vector3.Lerp(
                aimSprite.transform.localPosition,
                new Vector3(
                    0f,
                    aimSprite.transform.localPosition.y,
                    throwAccumulatedForce * throwForceMultiplier * throwAimForceCorrection
                ),
                1f - aimSpotLag
            );

            aimSprite.SetActive(true);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(
                // Adding 0.01f to Y to avoid Z-fight
                new Vector3[] {
                    transform.position + new Vector3(0f, 0.05f, 0f),
                    Vector3.Lerp(
                        transform.position + new Vector3(0f, 0.1f, 0f),
                        new Vector3( aimSprite.transform.position.x, transform.position.y+0.1f,  aimSprite.transform.position.z),
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
        aimSprite.transform.localPosition = new Vector3(0f, aimSprite.transform.localPosition.y, 0f);
        lineRenderer.positionCount = 0;
    }

    public Grabbable GetHeldObject()
    {
        return heldObject;
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
        foreach(var prop in objectsAtRange) 
        {
            if (prop == null) continue;
            if (IsWearing() && prop.GetProp() == hat.GetProp()) continue;
            if(prop.transform.position.y > bestHeight)
            {
                bestProp = prop;
                bestHeight = prop.transform.position.y;
            }
        }
        return bestProp;
    }

    Vector2 GetStickDirection(GamePadThumbSticks.StickValue val)
    {
        return new Vector2(val.X, val.Y);
    }

    bool IsWearing()
    {
        return hat != null;
    }

    public void Unwear()
    {
        hat.BecomeDropped();
        hat = null;
    }

    public void Wear(Grabbable prop)
    {
        if (IsWearing()) {
            Unwear();
        }
        var cos = prop.GetComponent<Cosmetic>();
        cos.BecomeWeared(headBone, color);

        // Update GM
        GameManager.instance.UpdatePlayer(
            index, 
            new GameManager.Player() {
                index = index,
                cosmetic = Library.instance.cosmetics.FindIndex(o => {
                    return o.GetComponent<Cosmetic>().uniqueID == cos.uniqueID; })
            }
        );

        hat = cos;
    }

    void GrabObject(Grabbable prop)
    {
        if (prop.IsCosmetic()) {
            Wear(prop);
            return;
        }

        float headHeight = 
            collider.height/2
            + prop.GetComponent<Collider>().bounds.extents.y/2
            + collider.bounds.center.y;

        prop.BecomeHeldBy(transform, new Vector3(0f, headHeight, 0f));
        heldObject = prop;
        // Visuals
        sweat.Activate();
        anim.SetBool("Carrying", true);
        SoundPlayer.PlayWithRandomPitch("fb_raccoon_grabbing", 0.5f);
    }

    public void DropHeldObject()
    {
        if(heldObject == null) return;

        heldObject.BecomeDropped();
        heldObject = null;
        // Visuals
        sweat.Desactivate();
        anim.SetBool("Carrying", false);
        // le son est trop proche du grab, je vais voir pour changer ça
        SoundPlayer.PlayWithRandomPitch("si_raccoon_droping_item", 0.5f);
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
        SoundPlayer.PlayWithRandomPitch("fb_raccoon_tossing", 0.2f);
    }

    public bool IsHolding()
    {
        return heldObject != null;
    }

    bool IsAnythingAtRange()
    {
        foreach(var prop in objectsAtRange.ToArray()) {
            ///////
            //  Removing non grabbable grabbable objects
            if (
                // Prop has disappeared
                prop == null || // or
                (
                    // This is my hat!
                    IsWearing() 
                    && prop.GetProp() == hat.GetProp()
                )   
                ||  // or
                (
                    // Hat is taken!
                    prop.IsCosmetic() &&
                    prop.GetComponent<Cosmetic>().IsWeared()
                )
            ){
                objectsAtRange.Remove(prop);
            }
            //
            ///////
        }
        return objectsAtRange.Count > 0;
    }
    #endregion
    
    public void Stun(float duration)
    {
        activated = false;
        duration = Mathf.Clamp(duration, 0f, 3f);
        anim.SetFloat("Speed", 0f);
        StartCoroutine(WaitAndWakeUp(duration));
    }

    IEnumerator WaitAndWakeUp(float time)
    {
        yield return new WaitForSeconds(time);
        activated = true;
    }

    public void Die()
    {
        DropHeldObject();
        activated = false;
        dead = true;
        anim.SetFloat("Speed", 0f);
    }

    public void KillPhysics()
    {
        rb.isKinematic = true;
        collider.isTrigger = true;
    }

    public bool IsDead()
    {
        return dead;
    }
}

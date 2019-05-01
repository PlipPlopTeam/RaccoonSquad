using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public new Collider collider;

    public int racoonValue = 1;
    public int humanValue = 1;
    [Range(0f, 200f)] public float weight = 10f;

    bool isHeld = false;

    private void Awake()
    {
        // Check if rigidbody has been referenced
        if(rigidbody == null) 
        {
            rigidbody = GetComponent<Rigidbody>();
            // If there is no rigidbody in this gameObject
            if(rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        if(collider == null) 
        {
            collider = GetComponent<Collider>();
            if(collider == null) collider = gameObject.AddComponent<BoxCollider>();
        }
    }

    public void BecomeHeldBy(Transform _transform, Vector3 offset=new Vector3())
    {
        transform.parent = _transform;
        transform.localPosition = offset;
        rigidbody.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        isHeld = true;
    }

    public void BecomeDropped()
    {
        transform.parent = null;
        rigidbody.isKinematic = false;
        gameObject.layer = 0; // Default layer
        isHeld = false;
    }

    public bool IsHeld()
    {
        return isHeld;
    }
}

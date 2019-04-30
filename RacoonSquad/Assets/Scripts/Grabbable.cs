using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    Rigidbody rb;
    public new Collider collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void GetHeldBy(Transform _transform, Vector3 offset=new Vector3())
    {
        transform.parent = _transform;
        transform.localPosition = offset;
        rb.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
    }

    public void GetDropped()
    {
        transform.parent = null;
        rb.isKinematic = false;
        gameObject.layer = 0; // Default layer
    }
}

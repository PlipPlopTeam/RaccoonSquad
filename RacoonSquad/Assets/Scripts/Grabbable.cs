using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public new Collider collider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void GetHeldBy(Transform _transform, Vector3 offset=new Vector3())
    {
        transform.parent = _transform;
        transform.localPosition = offset;
        rigidbody.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
    }

    public void GetDropped()
    {
        transform.parent = null;
        rigidbody.isKinematic = false;
        gameObject.layer = 0; // Default layer
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public int racoonValue = 1;
    public int humanValue = 1;
    [Range(0f, 200f)] public float weight = 10f;
    bool isHeld = false;

    Prop prop;

    private void Awake()
    {
        prop = GetComponent<Prop>();
        if(prop == null) Destroy(this);
    }

    public void BecomeHeldBy(Transform _transform, Vector3 offset=new Vector3())
    {
        transform.SetParent(_transform);
        transform.localPosition = offset;
        prop.rigidbody.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        isHeld = true;
    }

    public void BecomeDropped()
    {
        transform.parent = null;
        prop.rigidbody.isKinematic = false;
        gameObject.layer = 0; // Default layer
        isHeld = false;
    }

    public bool IsHeld()
    {
        return isHeld;
    }


    public Prop GetProp()
    {
        return prop;
	}
	
    public bool IsFlying()
    {
        if(prop.rigidbody.velocity.magnitude > 0.1f)
        {
            return true;
        }
        return false;
    }
}

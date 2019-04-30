using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventTransmitter : MonoBehaviour
{
    public event System.Action<Collider> onTriggerEnter;
    public event System.Action<Collider> onTriggerExit;
    public event System.Action<Collision> onColliderEnter;
    public event System.Action<Collision> onColliderExit;
    
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        onColliderEnter.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        onColliderExit.Invoke(collision);
    }
    
}

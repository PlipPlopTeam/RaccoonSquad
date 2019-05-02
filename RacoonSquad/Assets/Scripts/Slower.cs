using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slower : MonoBehaviour
{
    [Range(0f, 1f)]
    public float slow = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        MovementSpeed ms = other.gameObject.GetComponent<MovementSpeed>();
        if(ms != null) ms.AddSpeedModifier(slow, this.GetInstanceID());
    }

    void OnTriggerExit(Collider other)
    {
        MovementSpeed ms = other.gameObject.GetComponent<MovementSpeed>();
        if(ms != null) ms.RemoveSpeedModifier(this.GetInstanceID());
    }
}

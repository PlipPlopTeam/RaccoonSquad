using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusLook : MonoBehaviour
{
    public Transform head;
    public Vector3 adjustment;
    public float angleMax = 60f;
    public float speed = 0.1f;

    Transform transformTarget;
    Vector3 positionTarget;

    Vector3 targetDirection;

    void Awake()
    {
        if(head == null) Destroy(this);
    }

    void LateUpdate()
    {
        Vector3 direction = Vector3.zero;

        if(transformTarget != null) 
            direction = (transformTarget.position - head.position).normalized;
        else if(positionTarget != Vector3.zero) 
            direction = (positionTarget - head.position).normalized;

        if(Vector3.Angle(direction, transform.forward) > angleMax) 
            targetDirection = transform.forward;
        else 
            targetDirection = direction;

        head.forward = targetDirection;
        head.Rotate(adjustment);
    }

    // FOCUS ON
    public void FocusOn(Vector3 position)
    {
        LooseFocus();
        positionTarget = position;
    }
    public void FocusOn(Transform transform)
    {
        LooseFocus();
        transformTarget = transform;
    }

    // LOOSE FOCUS
    public void LooseFocus()
    {
        targetDirection = transform.forward;
        positionTarget = Vector3.zero;
        transformTarget = null;
    }
}
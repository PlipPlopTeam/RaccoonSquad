using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [HideInInspector] public new Rigidbody rigidbody;
    [HideInInspector] public new Collider collider;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if(collider == null) collider = gameObject.AddComponent<BoxCollider>();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

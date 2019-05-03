using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetic : MonoBehaviour, Prop.IProp
{
    public Vector3 offset;
    public Vector3 eulerRotationOffset;

    bool isBeingWeared;
    Prop prop;
    new Renderer renderer;

    private void Awake()
    {
        prop = GetComponent<Prop>();
        if (prop == null) Destroy(this);
        renderer = GetComponentInChildren<Renderer>();
        renderer.material = Instantiate(renderer.material);
    }

    private void Start()
    {
        prop.rigidbody.mass = 0.05f;
    }

    public void BecomeWeared(Transform _transform, Color color)
    {
        transform.SetParent(_transform);
        transform.localPosition = offset;
        transform.localEulerAngles = eulerRotationOffset;
        prop.rigidbody.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        renderer.material.color = color;
        isBeingWeared = true;
    }

    public void BecomeDropped()
    {
        transform.parent = null;
        prop.rigidbody.isKinematic = false;
        prop.transform.position += new Vector3(Mathf.Sin(Random.value), Mathf.Cos(Random.value), 0.1f);
        prop.rigidbody.AddForce(new Vector3(Random.value-0.5f, Random.value - 0.5f, Random.value - 0.5f));
        gameObject.layer = 0; // Default layer
        renderer.material.color = Color.white;
        isBeingWeared = false;
    }

    public bool IsWeared()
    {
        return isBeingWeared;
    }

    public Prop GetProp()
    {
        return prop;
    }
}

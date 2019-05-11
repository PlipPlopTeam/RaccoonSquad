using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prop : MonoBehaviour
{
    public interface IProp
    {
        Prop GetProp();
    }

    public System.Action<Collision> onHit;
    public float groundedThreshold = 0.01f;
    public bool isHuman;
    public int id = 0;

    [HideInInspector] public new Rigidbody rigidbody;
    [HideInInspector] public new Collider collider;

    public NavMeshObstacle obstacle;

    bool justHit;
    bool activated;
    
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if(collider == null) collider = gameObject.AddComponent<BoxCollider>();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
        if (obstacle == null) obstacle = gameObject.AddComponent<NavMeshObstacle>();

        activated = false;
        StartCoroutine(WaitBeforeActivate());
    }

    IEnumerator WaitBeforeActivate()
    {
        yield return new WaitForSeconds(1f);
        activated = true;
    }
    
    public bool IsGrounded()
    {
        return Mathf.Round(rigidbody.velocity.y / groundedThreshold) == 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(onHit == null) return;
        
        if ((IsGrounded() || collision.collider.name == "Ground") && onHit.GetInvocationList().Length > 0) {
            if (activated) onHit.Invoke(collision);
        }
    }
}

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

    public enum PropType { PROP, HUMAN_STATUE, HUMAN_WAYPOINT};

    public PropType propType;
    public System.Action<Collision> onHit;
    public float groundedThreshold = 0.01f;
    public bool isObstacle = true;
    public int id = 0;

    [HideInInspector] public new Rigidbody rigidbody;
    [HideInInspector] public new Collider collider;

    NavMeshObstacle obstacle;
    bool justHit;
    bool activated;
    
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        obstacle = GetComponent<NavMeshObstacle>();
        if (collider == null) collider = gameObject.AddComponent<BoxCollider>();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
        if (isObstacle && obstacle == null) 
        {
            obstacle = gameObject.AddComponent<NavMeshObstacle>();
            // collider.bounds.size return a local scale value
            // So if transform scale of the game object is != 1 it causes problem
            obstacle.size = collider.bounds.size;
            // when strangly collider.bounds.center is world scaled 🤔
            obstacle.center = collider.bounds.center - transform.position;
        }

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

    public NavMeshObstacle GetObstacle()
    {
        if (!isObstacle) throw new System.Exception();
        return obstacle;
    }
}

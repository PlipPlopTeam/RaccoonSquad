using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HumanState{ Walking, Thinking, Chasing, Collecting }

public class HumanBehavior : MonoBehaviour
{
    [Header("State")]
    public HumanState state;

    [Header("Settings")]
    public float walkSpeed;
    public float chaseSpeed;
    public float velocityLerpSpeed = 1f;
    public float navTreshold = 1f;
    public List<Transform> paths;

    [Header("Bones")]
    public Transform handBone;

    // Variables
    List<GameObject> inRange = new List<GameObject>();
    int currentWaypoint;
    float targetSpeed;
    // Referencies
    GameObject mark;
    NavMeshAgent agent;
    Sight sight;
    FocusLook look;
    Animator anim;
    CollisionEventTransmitter rangeEvent;
    PlayerController seenPlayer;
    Grabbable seenItem;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<Sight>();
        anim = GetComponent<Animator>();
        look = GetComponent<FocusLook>();

        // Check which objects are currently grabbable
        rangeEvent = GetComponentInChildren<CollisionEventTransmitter>();
        rangeEvent.onTriggerEnter += (Collider other) => { inRange.Add(other.transform.gameObject); };
        rangeEvent.onTriggerExit += (Collider other) => { inRange.Remove(other.transform.gameObject); };
    }

    void ChangeState(HumanState newState)
    {
        switch(newState)
        {
            case HumanState.Walking:
                targetSpeed = walkSpeed;
                break;
            case HumanState.Thinking:
                targetSpeed = 0;
                break;
            case HumanState.Chasing:
                targetSpeed = chaseSpeed;
                break;
            case HumanState.Collecting:
                seenPlayer = null;
                break;
        }

        state = newState;
    }

    void Start()
    {
        ChangeState(HumanState.Walking);
        if (paths.Count > 0) MoveTo(0);
        else {
            // Creates dummy GO for pathfinding
            paths.Add(Instantiate<GameObject>(new GameObject(), transform.position, Quaternion.identity).transform);
        }
    }
    
    void Update()
    {
        // Lerp agent speed for a more organic effect
        agent.speed = Mathf.Lerp(agent.speed, targetSpeed, velocityLerpSpeed * Time.deltaTime);
        anim.SetFloat("Speed", agent.speed/chaseSpeed);

        // Different update depending on the current state
        StateUpdate();

        // Check if the human has reach his destination
        if(Vector3.Distance(transform.position, agent.destination) < navTreshold) StateDestinationReached();
    }

    bool IsObjectInRange(GameObject obj)
    {
        foreach(GameObject o in inRange)
        {
            if(o == obj) return true;
        }
        return false;
    }

    void StateUpdate()
    {
        CleanSeenItem();
        switch(state)
        {
            case HumanState.Walking:
                ScanRacoons();
                break;

            case HumanState.Chasing:
                agent.destination = seenPlayer.transform.position;
                if(seenPlayer.GetHeldObject() == null) ChangeState(HumanState.Collecting);
                break;

            case HumanState.Collecting:
                if(IsObjectInRange(seenItem.gameObject) && !seenItem.IsFlying()) StartCoroutine(PickUp(seenItem.gameObject));
                else agent.destination = seenItem.transform.position;
                break;
        }
    }
    void CleanSeenItem()
    {
        // Security fallback
        if (seenItem == null && seenPlayer == null) {
            ChangeState(HumanState.Walking);
        }
    }
    void StateDestinationReached()
    {
        switch(state)
        {
            case HumanState.Walking:
                MoveTo(GetNextWaypoint());
                break;

            case HumanState.Chasing:
                //seenPlayer.DropHeldObject();
                MoveTo(GetNextWaypoint());
                break;
        }
    }

    IEnumerator PickUp(GameObject obj)
    {
        // Look at the object
        Vector3 direction = obj.transform.position - transform.position;
        direction = new Vector3(direction.x, transform.position.y, direction.z);
        transform.forward = direction;
        look.LooseFocus();
        
        Grabbable grab = obj.GetComponent<Grabbable>();
        if(grab != null) grab.BecomeHeldBy(handBone);
        ChangeState(HumanState.Thinking);
        anim.SetTrigger("Pickup");
        
        yield return new WaitForSeconds(1f);

        Destroy(seenItem.gameObject);
        // Return to normal state
        ChangeState(HumanState.Walking);
        if(paths.Count > 0) MoveTo(0);
    }


    IEnumerator Suprised(Vector3 position)
    {
        // Look at the intresting thing
        Vector3 direction = position - transform.position;
        direction = new Vector3(direction.x, transform.position.y, direction.z);
        transform.forward = direction;

        // Stops agent movement
        agent.destination = transform.position;
        anim.SetTrigger("Suprised");

        // Put a mark on his head
        Mark();

        ChangeState(HumanState.Thinking);
        // Trigger Animation
        
        yield return new WaitForSeconds(1f);

        Unmark();
        
        if (seenPlayer == null) {
            yield break;
        }

        seenItem = seenPlayer.GetHeldObject();
        if(seenItem != null)
        {
            look.FocusOn(seenItem.transform);
            ChangeState(HumanState.Chasing);
        }
        else
        {
            ChangeState(HumanState.Walking);
            if(paths.Count > 0) MoveTo(0);
        }
        // Return to normal state or chasing
    }

    // EXCLAMATION MARK ABOVE HEAD
    void Mark()
    {
        if(mark != null) Destroy(mark);
        mark = Instantiate(Library.instance.exclamationMarkPrefab, transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity, transform);
    }
    void Unmark()
    {
        if(mark != null) Destroy(mark);
    }

    void ScanRacoons()
    {
        GameObject[] seens = sight.Scan();
        foreach(GameObject go in seens)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if(pc != null)
            {
                SpotRaccoon(pc);
            }
        }
    }

    void SpotRaccoon(PlayerController pc)
    {
        seenPlayer = pc;
        look.FocusOn(seenPlayer.transform);
        StartCoroutine(Suprised(seenPlayer.transform.position));
    }

    int GetNextWaypoint()
    {
        int waypoint = currentWaypoint + 1;
        if(waypoint >= paths.Count) waypoint = 0;
        return waypoint;
    }

    void MoveTo(int waypointIndex)
    {
        if(paths.Count == 0) return;
        agent.destination = paths[waypointIndex].position;
        currentWaypoint = waypointIndex;
    }
}

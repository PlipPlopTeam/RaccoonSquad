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

    [Header("Path")]
    public Transform[] paths;

    [Header("Bones")]
    public Transform headBone;

    // Variables
    int currentWaypoint;
    float targetSpeed;
    GameObject mark;
    // Referencies
    NavMeshAgent agent;
    Sight sight;
    Animator anim;

    PlayerController seenPlayer;
    Grabbable seenItem;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<Sight>();
        anim = GetComponent<Animator>();
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
        if(paths.Length > 0) MoveTo(0);
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

    void LateUpdate()
    {
        Vector3 lookDirection = Vector3.zero;

        if(seenPlayer != null) lookDirection = (seenPlayer.transform.position - headBone.transform.position).normalized;
        else if(seenItem != null) lookDirection = (headBone.transform.position - headBone.transform.position).normalized;

        if(lookDirection != Vector3.zero)
        {
            if(Vector3.Angle(lookDirection, transform.forward) < 60f)
            {
                headBone.forward = lookDirection;
                headBone.Rotate(new Vector3(0f, 0f, -90f));
            }
        }
    }


    void StateUpdate()
    {
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
                agent.destination = seenItem.transform.position;
                break;
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

            case HumanState.Collecting:
                Destroy(seenItem.gameObject);
                MoveTo(GetNextWaypoint());
                ChangeState(HumanState.Walking);
                break;
        }
    }

    IEnumerator Suprised(Vector3 position)
    {
        // Look at the intresting thing
        Vector3 direction = seenPlayer.transform.position - transform.position;
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

        seenItem = seenPlayer.GetHeldObject();
        if(seenItem != null)
        {
            ChangeState(HumanState.Chasing);
        }
        else
        {
            ChangeState(HumanState.Walking);
            if(paths.Length > 0) MoveTo(0);
        }
        // Return to normal state or chasing
    }

    // EXCLAMATION MARK ABOVE HEAD
    void Mark()
    {
        if(mark != null) Destroy(mark);
        mark = Instantiate(Library.instance.exclamationMarkPrefab, transform);
        mark.transform.localPosition = new Vector3(0f, 5.25f, 0f);
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
        StartCoroutine(Suprised(seenPlayer.transform.position));
    }

    int GetNextWaypoint()
    {
        int waypoint = currentWaypoint + 1;
        if(waypoint >= paths.Length) waypoint = 0;
        return waypoint;
    }

    void MoveTo(int waypointIndex)
    {
        if(paths.Length == 0) return;
        agent.destination = paths[waypointIndex].position;
        currentWaypoint = waypointIndex;
    }
}

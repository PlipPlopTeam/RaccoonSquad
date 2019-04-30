using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HumanState{ Walking, Chasing }

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

    // Variables
    int currentWaypoint;
    float targetSpeed;
    GameObject mark;
    // Referencies
    NavMeshAgent agent;
    Sight sight;
    PlayerController seenPlayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<Sight>();
    }

    void ChangeState(HumanState newState)
    {
        switch(newState)
        {
            case HumanState.Walking:
                targetSpeed = walkSpeed;
                break;
            case HumanState.Chasing:
                targetSpeed = chaseSpeed;
                break;
        }

        state = newState;
    }

    void Mark()
    {
        if(mark != null) Destroy(mark);
        mark = Instantiate(Library.instance.exclamationMarkPrefab, transform);
        mark.transform.localPosition = new Vector3(0f, 5.25f, 0f);
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

        switch(state)
        {
            case HumanState.Walking:
                // Check if the Human as reach his destination
                if(Vector3.Distance(transform.position, agent.destination) < navTreshold) MoveTo(GetNextWaypoint());
                // Scanning for Racoons
                ScanRacoons();
                break;
            case HumanState.Chasing:
                break;
        }
    }

    void ScanRacoons()
    {
        GameObject[] seens = sight.Scan();
        foreach(GameObject go in seens)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if(pc != null)
            {
                seenPlayer = pc;
                agent.destination = pc.transform.position;
                ChangeState(HumanState.Chasing);
            }
        }
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

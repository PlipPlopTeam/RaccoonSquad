using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum HumanState{ Calm, Aware, Angry }
enum HumanAction{ Roaming, Chasing }

public class HumanBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float walkSpeed;
    public float chaseSpeed;
    public float velocityLerpSpeed = 1f;
    public float navTreshold = 1f;

    [Header("Path")]
    public Transform[] paths;
    int currentWaypoint;

    float targetSpeed;

    // Referencies
    NavMeshAgent agent;
    Sight sight;
    PlayerController seenPlayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<Sight>();
    }

    void ChangeState(HumanState state)
    {
        switch(state)
        {
            case HumanState.Calm:
                targetSpeed = walkSpeed;
                break;
            case HumanState.Aware:
                targetSpeed = walkSpeed;
                break;
            case HumanState.Angry:
                targetSpeed = chaseSpeed;
                break;
        }
    }

    void Start()
    {
        ChangeState(HumanState.Calm);

        if(paths.Length > 0)
        {
            MoveTo(0);
        }
    }
    
    void Update()
    {
        // Lerp agent speed for a more organic effect
        agent.speed = Mathf.Lerp(agent.speed, targetSpeed, velocityLerpSpeed * Time.deltaTime);

        // Check if the Human as reach his destination
        if(Vector3.Distance(transform.position, agent.destination) < navTreshold)
        {
            MoveTo(GetNextWaypoint());
        }

        ScanRacoons();
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
                ChangeState(HumanState.Angry);
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

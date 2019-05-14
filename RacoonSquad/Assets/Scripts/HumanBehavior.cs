using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HumanState{ Walking, Thinking, Chasing, Collecting }

public class HumanBehavior : MonoBehaviour
{
    [Header("State")]
    public HumanState state;

    [Header("Movement")]
    public float walkSpeed;
    public float chaseSpeed;
    public float velocityLerpSpeed = 1f;
    public float navTreshold = 1f;
    public List<Transform> path;

    [Header("Settings")]
    public float reactionTime = 1f;
    public float forgetTime = 5f;
    public float minStunDuration = 0.5f;
    public float maxStunDuration = 2f;
    public float stunVelocityTreshold = 1f;

    [Header("Bones")]
    public Transform headBone;
    public Transform handBone;

    // Variables
    List<GameObject> inRange = new List<GameObject>();
    bool stun;
    bool stunProof = false;
    int currentWaypoint;
    float targetSpeed;
    float currentSpeed;
    // Referencies
    NavMeshAgent agent;
    Sight sight;
    Hearing ear;
    EmotionRenderer emotion;
    MovementSpeed movementSpeed;
    FocusLook look;
    Animator anim;
    CollisionEventTransmitter rangeEvent;
    PlayerController seenPlayer;
    PlayerController lastSeenPlayer;
    Grabbable seenItem;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<Sight>();
        anim = GetComponent<Animator>();
        look = GetComponent<FocusLook>();
        emotion = GetComponent<EmotionRenderer>();

        ear = GetComponent<Hearing>();
        ear.heard += (Vector3 position) => { this.OnHeard(position); };

        movementSpeed = gameObject.AddComponent<MovementSpeed>();

        // Check which objects are currently grabbable
        rangeEvent = GetComponentInChildren<CollisionEventTransmitter>();
        rangeEvent.onTriggerEnter += (Collider other) => { inRange.Add(other.transform.gameObject); };
        rangeEvent.onTriggerExit += (Collider other) => { inRange.Remove(other.transform.gameObject); };
    }


    private void FindPath()
    {
        // Get reference to points
        List<Transform> waypoints = new List<Transform>();
        foreach(Waypoint wp in  FindObjectsOfType<Waypoint>()) waypoints.Add(wp.transform);

        // If there is no waypoints in the scene
        if(waypoints.Count == 0)
        {
            path.Add(Instantiate<GameObject>(new GameObject(), transform.position, Quaternion.identity).transform);
        }
        
        // Apply the path to the actor
        path = waypoints;
    }
    
    void Start()
    {
        if(path.Count <= 0) FindPath();
        ChangeState(HumanState.Walking);
    }

    void OnHeard(Vector3 position)
    {   
        if(state == HumanState.Walking) StartCoroutine(HearSomething(position));
    }
    IEnumerator HearSomething(Vector3 where)
    {
        SuprisedBy(where);
        emotion.Show("Think");
        
        yield return new WaitForSeconds(reactionTime);

        emotion.Hide();
        if(state == HumanState.Thinking) ChangeState(HumanState.Walking);
    }

    void ChangeState(HumanState newState)
    {
        switch(newState)
        {
            case HumanState.Walking:
                seenItem = null;
                seenPlayer = null;
                targetSpeed = walkSpeed;
                look.LooseFocus();
                emotion.Hide();
                MoveTo(currentWaypoint);
                break;
            case HumanState.Thinking:
                targetSpeed = 0;
                SoundPlayer.PlayAtPosition("si_concerned_human", transform.position, 0.1f, true);
                break;
            case HumanState.Chasing:
                emotion.Show("Suprised");
                targetSpeed = chaseSpeed;
                SoundPlayer.PlayAtPosition("si_raccoon_spotted", transform.position, 0.1f, false);
                break;
            case HumanState.Collecting:
                seenPlayer = null;
                break;
        }
        state = newState;
    }

    
    void Update()
    {
        // Lerp agent speed for a more organic effect
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, velocityLerpSpeed * Time.deltaTime);
        agent.speed = currentSpeed * movementSpeed.GetMultiplier();
        anim.SetFloat("Speed", (agent.velocity.magnitude)/(chaseSpeed-walkSpeed));

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
        if(stun) return;

        CleanSeenItem();
        switch(state)
        {
            case HumanState.Walking:
                ScanRacoons();
                break;

            case HumanState.Chasing:
                agent.destination = seenPlayer.transform.position;
                if(seenPlayer.GetHeldObject() == null) 
                {
                    if(seenItem != null)
                    {
                        look.FocusOn(seenItem.transform);
                        ChangeState(HumanState.Collecting);
                    }
                    else 
                    {
                        ChangeState(HumanState.Walking);
                    }
                }
                else
                {
                    if(IsObjectInRange(seenPlayer.gameObject) && !seenPlayer.IsInvincible()) 
                    {

                        HangRaccoon();
                        
                        // Effects
                        look.LooseFocus();
                        ear.enabled = false;
                        CameraController.instance.FocusOn(headBone, 30f);
                        // System
                        ChangeState(HumanState.Walking);
                        GameManager.instance.GameOver();
                    }
                }
                break;

            case HumanState.Collecting:
                if(agent.velocity.magnitude > 0.01f) 
                {
                    agent.destination = seenItem.transform.position;
                    if(IsObjectInRange(seenItem.gameObject) && !seenItem.IsFlying()) StartCoroutine(PickUp(seenItem.gameObject));                 
                }
                else
                {
                    ChangeState(HumanState.Walking);
                }
                break;
        }
    }

    void HangRaccoon()
    {
        anim.SetBool("Carrying", true);
        // Hanging player
        seenPlayer.Die();
        //seenPlayer.KillPhysics();
        seenPlayer.KillConstraints();
        seenPlayer.Hang(); 
        //seenPlayer.gameObject.transform.SetParent(handBone);
        seenPlayer.gameObject.transform.rotation = new Quaternion();
        seenPlayer.gameObject.transform.position = handBone.position;
        seenPlayer.gameObject.transform.forward = transform.forward;
        seenPlayer.gameObject.transform.up = -Vector3.up;
        seenPlayer.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        Rigidbody handRb = handBone.gameObject.AddComponent<Rigidbody>();
        handRb.constraints = RigidbodyConstraints.FreezeRotationX 
        | RigidbodyConstraints.FreezeRotationY 
        | RigidbodyConstraints.FreezeRotationZ 
        | RigidbodyConstraints.FreezePositionX 
        | RigidbodyConstraints.FreezePositionY 
        | RigidbodyConstraints.FreezePositionZ;
        seenPlayer.gameObject.AddComponent<CharacterJoint>().connectedBody = handRb;
        NavMeshObstacle nmo = seenPlayer.gameObject.GetComponent<NavMeshObstacle>();
        if(nmo != null) Destroy(nmo);

        stunProof = true;
    }

    void CleanSeenItem()
    {
        if(seenItem == null && seenPlayer == null) ChangeState(HumanState.Walking);
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


        if(seenItem != null && seenItem.gameObject != null) Destroy(seenItem.gameObject);
        // Return to normal state
        ChangeState(HumanState.Walking);
        if(path.Count > 0) MoveTo(0);
    }

    void SuprisedBy(Vector3 position)
    {
        // Look at the intresting thing
        Vector3 direction = position - transform.position;
        direction = new Vector3(direction.x, transform.position.y, direction.z);
        transform.forward = direction;
       // SoundPlayer.PlayAtPosition("si_lured_human", transform.position, 0.2f, true);

        // Stops agent movement
        agent.destination = transform.position;
        anim.SetTrigger("Suprised");

        ChangeState(HumanState.Thinking);
        // Trigger Animation
    }

    void ScanRacoons()
    {
        GameObject[] seens = sight.Scan();
        foreach(GameObject go in seens)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if(pc != null && !pc.hidden && !pc.IsDead()) 
            {
                StartCoroutine(SpotRaccoon(pc));
                break;
            }
        }
    }

    IEnumerator SpotRaccoon(PlayerController pc)
    {
        seenPlayer = pc;
        look.FocusOn(seenPlayer.transform);
        if(seenPlayer != lastSeenPlayer) RememberPlayer(seenPlayer);
        else if(seenPlayer.GetHeldObject() == null) yield break;
        SuprisedBy(seenPlayer.transform.position);
        emotion.Show("Think");
        
        yield return new WaitForSeconds(reactionTime);

        emotion.Hide();

        if (seenPlayer == null || stun) yield break;

        seenItem = seenPlayer.GetHeldObject();
        if(seenItem != null) ChangeState(HumanState.Chasing);
        else ChangeState(HumanState.Walking);
    }

    int GetNextWaypoint()
    {
        int waypoint = currentWaypoint + 1;
        if(waypoint >= path.Count) waypoint = 0;
        return waypoint;
    }

    void MoveTo(int waypointIndex)
    {
        if(path.Count == 0) return;
        agent.destination = path[waypointIndex].position;
        currentWaypoint = waypointIndex;
    }

    IEnumerator WaitAndForgetPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        lastSeenPlayer = null;
    }

    void RememberPlayer(PlayerController pc)
    {
        lastSeenPlayer = pc;
        StartCoroutine(WaitAndForgetPlayer(forgetTime));
    }

    public void Stun(float duration)
    {
        if(stunProof) return;

        look.LooseFocus();
        anim.SetTrigger("Hit");
        emotion.Show("Dizzy");
        SoundPlayer.PlayAtPosition("si_stunned_human", transform.position, 0.1f, true);
        ChangeState(HumanState.Thinking);
        agent.destination = transform.position;
        duration = Mathf.Clamp(duration, minStunDuration, maxStunDuration);
        StartCoroutine(WaitAndWakeUp(duration));
        stun = true;
    }

    IEnumerator WaitAndWakeUp(float time)
    {
        yield return new WaitForSeconds(time);
        emotion.Hide();
        ChangeState(HumanState.Walking);
        stun = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if(rb != null && rb.velocity.magnitude > stunVelocityTreshold) Stun(rb.velocity.magnitude);
    }
}

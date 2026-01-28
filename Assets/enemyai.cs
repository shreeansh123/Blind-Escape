using UnityEngine;
using UnityEngine.AI;

public class BlindEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public NavMeshAgent agent;
    public Transform worldSpawn;
    public float patrolSpeed = 2f;
    public float huntSpeed = 6f;
    public float patrolRadius = 10f;

    [Header("Behavior")]
    public float reactionDelay = 0.5f;
    public float chaseMemoryDuration = 4f; //time enemy runs after goggle is removed

    
    private Vector3 lastNoiseLocation;
    private float lastHeardTime;           
    private bool isHunting = false;
    private bool shouldPatrol = true;

    void Start()
    {
        agent.speed = patrolSpeed;
        if (shouldPatrol) SetRandomDestination();
    }

    void OnEnable() { GoggleSystem.OnGoggleNoise += OnHeardSound; }
    void OnDisable() { GoggleSystem.OnGoggleNoise -= OnHeardSound; }

    void OnHeardSound(Vector3 noisePosition)
    {
        lastNoiseLocation = noisePosition;
        lastHeardTime = Time.time; // reset time

        shouldPatrol = false;

        if (isHunting)
        {
            agent.SetDestination(noisePosition);
        }
        else if (!IsInvoking("StartHunting"))
        {
            Invoke("StartHunting", reactionDelay);
        }
    }

    void StartHunting()
    {
        isHunting = true;
        agent.speed = huntSpeed;
        agent.SetDestination(lastNoiseLocation);
    }

    void Update()
    {
        
        if (isHunting && Time.time > lastHeardTime + chaseMemoryDuration)
        {
            isHunting = false;
            agent.ResetPath(); // stop moving so they dont bunch up at one place
            return; 
        }

        
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (isHunting)
            {
                isHunting = false;
                agent.ResetPath(); 
            }
            else if (shouldPatrol)
            {
                SetRandomDestination();
            }
        }
    }

    
    
    void SetRandomDestination()
    {
        if (isHunting) return;

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            if (worldSpawn != null)
            {
                other.transform.position = worldSpawn.position; //respawn player if died
                other.transform.rotation = worldSpawn.rotation;
            }

            if (cc != null) cc.enabled = true;

            isHunting = false;
            shouldPatrol = true; 
            agent.speed = patrolSpeed;
            agent.ResetPath();
            SetRandomDestination();
        }
    }
}
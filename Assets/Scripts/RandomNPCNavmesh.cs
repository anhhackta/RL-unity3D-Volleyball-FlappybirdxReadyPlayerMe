using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
public class RandomNPCWithVoice : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveRadius = 10f;
    [SerializeField] private float waitTimeMin = 2f;
    [SerializeField] private float waitTimeMax = 5f;
    
    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float runChance = 0.3f;

    [Header("Animation Settings")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    
    [Header("Voice Interaction")]
    [SerializeField] private AudioClip[] voiceClips;
    [SerializeField] private float voiceCooldown = 5f;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private bool useVisualDebug = true;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private Transform player;

    private float waitTimer;
    private bool isMoving = false;
    private float voiceTimer = 0f;

    private void Start()
    {
        InitializeComponents();
        SetRandomWaitTime();
        MoveToRandomPoint();

        // Find player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animatorController != null && animator != null)
            animator.runtimeAnimatorController = animatorController;

        agent.speed = walkSpeed;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    private void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;
        HandleMovement();
        UpdateAnimation();
        HandleVoiceInteraction();
    }

    private void HandleMovement()
    {
        float velocity = agent.velocity.magnitude;
        isMoving = velocity > 0.1f;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                MoveToRandomPoint();
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        bool walking = agent.velocity.magnitude > 0.1f;
        animator.SetBool(IsWalkingHash, walking);
    }

    private void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius + transform.position;
        randomDirection.y = transform.position.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, moveRadius, NavMesh.AllAreas))
        {
            bool shouldRun = Random.value < runChance;
            agent.speed = shouldRun ? runSpeed : walkSpeed;
            agent.SetDestination(hit.position);
            isMoving = true;
        }

        SetRandomWaitTime();
    }

    private void SetRandomWaitTime()
    {
        waitTimer = Random.Range(waitTimeMin, waitTimeMax);
    }

    // ----------------- Voice Interaction -----------------
    private void HandleVoiceInteraction()
    {
        if (player == null || voiceClips.Length == 0) return;

        voiceTimer -= Time.deltaTime;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRadius && voiceTimer <= 0f)
        {
            PlayRandomVoice();
            voiceTimer = voiceCooldown;
        }
    }

    private void PlayRandomVoice()
    {
        int index = Random.Range(0, voiceClips.Length);
        audioSource.PlayOneShot(voiceClips[index]);
    }

    // Public methods
    public void SetVoiceSettings(float cooldown, float radius)
    {
        voiceCooldown = cooldown;
        detectionRadius = radius;
    }

    public void AddVoiceClip(AudioClip clip)
    {
        var list = new System.Collections.Generic.List<AudioClip>(voiceClips);
        list.Add(clip);
        voiceClips = list.ToArray();
    }

    public void TriggerVoiceNow()
    {
        PlayRandomVoice();
        voiceTimer = voiceCooldown;
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        if (!useVisualDebug) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, moveRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}

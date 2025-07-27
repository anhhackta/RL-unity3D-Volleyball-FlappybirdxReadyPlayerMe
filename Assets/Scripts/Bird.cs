using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Bird : Agent
{
    private const float COLLIDER_DISTANCE = 2.0f;

    private Rigidbody2D _rigidbody;
    private bool _isPressed;
    [SerializeField] private float _forceFactor = 4;

    public PipeSet pipes;
    public float counter;
    
    [Header("Audio")]
    public AudioClip flapSound;
    public AudioClip hitSound;
    public AudioClip pointSound;
    
    private bool isAIMode = false;
    private bool gameStarted = false;
    private bool gamePaused = false;
    private int lastScore = 0;
    private AudioSource audioSource;

    private void Update()
    {
        if (gameStarted && !gamePaused)
        {
            counter += Time.deltaTime;
            
            // Check for score increase
            int currentScore = Mathf.FloorToInt(counter / 2f);
            if (currentScore > lastScore)
            {
                lastScore = currentScore;
                PlaySound(pointSound);
            }
            
            // Handle player input
            if (!isAIMode)
            {
                HandlePlayerInput();
            }
        }
    }
    
    private void HandlePlayerInput()
    {
        bool inputPressed = Input.GetMouseButtonDown(0) || 
                           Input.GetKeyDown(KeyCode.Space) || 
                           Input.GetKeyDown(KeyCode.UpArrow);
        
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                inputPressed = true;
            }
        }
        
        if (inputPressed && !_isPressed)
        {
            UpwardForce();
            _isPressed = true;
        }
        
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            _isPressed = false;
        }
    }
    
    public void SetGameMode(bool aiMode)
    {
        isAIMode = aiMode;
    }
    
    public void StartGame()
    {
        gameStarted = true;
        gamePaused = false;
        _rigidbody.gravityScale = 1;  // Enable physics
        OnEpisodeBegin();
    }
    
    public void RestartGame()
    {
        gameStarted = true;
        gamePaused = false;
        _rigidbody.gravityScale = 1;  // Enable physics
        OnEpisodeBegin();
    }
    
    public void StopGame()
    {
        gameStarted = false;
        gamePaused = true;
        _rigidbody.gravityScale = 0;  // Disable physics
        _rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Freeze bird ban đầu
        _rigidbody.gravityScale = 0;
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void UpwardForce()
    {
        _rigidbody.AddForce(Vector2.up * _forceFactor, ForceMode2D.Impulse);
        PlaySound(flapSound);
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (isAIMode && gameStarted && !gamePaused)
        {
            AddReward(Time.fixedDeltaTime);

            int tap = Mathf.FloorToInt(actions.DiscreteActions[0]);

            if (tap == 0)
            {
                _isPressed = false;
            }
            if (tap == 1 && !_isPressed)
            {
                UpwardForce();
                _isPressed = true;
            }
        }
    }
    
    public override void CollectObservations(VectorSensor vs)
    {
        Vector2 nextPipePos = pipes.GetNextPipe().localPosition;
        float vel = Mathf.Clamp(_rigidbody.linearVelocity.y, -COLLIDER_DISTANCE, COLLIDER_DISTANCE);

        vs.AddObservation(transform.localPosition.y / COLLIDER_DISTANCE);
        vs.AddObservation(vel / COLLIDER_DISTANCE);
        vs.AddObservation(nextPipePos.y / COLLIDER_DISTANCE);
        vs.AddObservation(nextPipePos.x);
        vs.AddObservation(_isPressed ? 1f : -1f);
    }
    
    public override void OnEpisodeBegin()
    {
        _rigidbody.linearVelocity = Vector2.zero;
        transform.localPosition = Vector2.zero;
        counter = 0f;
        lastScore = 0;
        pipes.ResetPosition();
        _isPressed = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameStarted && !gamePaused)
        {
            PlaySound(hitSound);
            
            if (isAIMode)
            {
                SetReward(-1);
                EndEpisode();
            }
            else
            {
                gameStarted = false;
                gamePaused = true;
                GameManager.Instance.GameOver();
            }
        }
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        bool jumpInput = Input.GetKey(KeyCode.Space) || 
                        Input.GetKey(KeyCode.UpArrow) || 
                        Input.GetMouseButton(0);
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                jumpInput = true;
            }
        }
        
        actionsOut.DiscreteActions.Array[0] = jumpInput ? 1 : 0;
    }
}

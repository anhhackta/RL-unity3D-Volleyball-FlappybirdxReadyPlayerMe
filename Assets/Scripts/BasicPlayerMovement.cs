using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runMultiplier = 2f;
    public float jumpForce = 5f;
    
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isRunning = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }
    
    public void Move(float horizontal, float vertical)
    {
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        float currentSpeed = isRunning ? moveSpeed * runMultiplier : moveSpeed;
        
        if (direction.magnitude >= 0.1f)
        {
            Vector3 movement = direction * currentSpeed;
            movement.y = rb.linearVelocity.y; // Preserve Y velocity for gravity
            rb.linearVelocity = movement;
        }
        else
        {
            // Stop horizontal movement when no input
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    
    public void SetIsRunning(bool running)
    {
        isRunning = running;
    }
    
    public void TryJump()
    {
        if (isGrounded && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if touching ground
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.name.ToLower().Contains("plane") ||
            collision.gameObject.name.ToLower().Contains("ground") ||
            collision.contacts[0].normal.y > 0.5f) // Surface is mostly horizontal
        {
            isGrounded = true;
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        // Stay grounded while touching ground
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.name.ToLower().Contains("plane") ||
            collision.gameObject.name.ToLower().Contains("ground") ||
            collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}

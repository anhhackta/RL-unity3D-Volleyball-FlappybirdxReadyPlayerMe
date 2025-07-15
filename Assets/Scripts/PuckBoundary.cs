using UnityEngine;

public class PuckBoundary : MonoBehaviour
{
    [Header("Boundary Settings")]
    public float minX = -2.5f;
    public float maxX = 2.5f;
    public float minZ = -2.5f;
    public float maxZ = 2.5f;
    public float minY = 0.3f; // Độ cao tối thiểu
    public float resetY = 0.5f; // Độ cao reset
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        // Kiểm tra giới hạn
        Vector3 pos = transform.position;
        bool needReset = false;
        
        // Giới hạn X
        if (pos.x < minX || pos.x > maxX)
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            needReset = true;
        }
        
        // Giới hạn Z
        if (pos.z < minZ || pos.z > maxZ)
        {
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            needReset = true;
        }
        
        // Giới hạn Y (không cho rơi xuống)
        if (pos.y < minY)
        {
            pos.y = resetY;
            needReset = true;
        }
        
        if (needReset)
        {
            transform.position = pos;
            
            // Reset velocity nếu puck bị kẹt
            if (rb != null)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = 0; // Không cho puck bay lên cao
                rb.linearVelocity = vel;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Nếu có collider trigger làm boundary, reset puck về giữa
        if (other.CompareTag("Boundary"))
        {
            transform.position = new Vector3(0, resetY, 0);
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}

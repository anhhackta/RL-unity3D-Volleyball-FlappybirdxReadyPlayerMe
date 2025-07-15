using UnityEngine;

public class AirHockeyPaddle : MonoBehaviour
{
    public float speed = 10f;
    public bool isPlayer1; // True cho Paddle 1, False cho Paddle 2
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"Paddle '{gameObject.name}' thiếu component Rigidbody! Hãy thêm Rigidbody vào paddle.");
        }
    }

    void Update()
    {
        // Chỉ cho phép điều khiển khi là Player 1 và không có AI agent điều khiển
        if (!isPlayer1) return;
        
        // Kiểm tra xem có AirHockeyAgent nào đang enabled không
        AirHockeyAgent agent = GetComponent<AirHockeyAgent>();
        if (agent != null && agent.enabled) return; // AI đang điều khiển

        if (rb == null) return; // Bảo vệ khỏi lỗi thiếu Rigidbody

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        // Giới hạn di chuyển
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -1f, 1f);
        pos.z = isPlayer1 ? Mathf.Clamp(pos.z, -0.614793f, 0f) : Mathf.Clamp(pos.z, 0f, 0.614793f);
        transform.position = pos;
    }
}
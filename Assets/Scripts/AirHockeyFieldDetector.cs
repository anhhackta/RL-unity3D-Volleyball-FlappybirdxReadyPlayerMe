using UnityEngine;

public class AirHockeyFieldDetector : MonoBehaviour
{
    [Header("Field Detection")]
    public Transform fieldPlane; // Kéo plane sân vào đây
    public Transform[] goals; // Kéo 2 goals vào đây
    
    [Header("Field Info (Auto-calculated)")]
    [SerializeField] private Vector3 fieldSize; // Kích thước thực tế của sân
    [SerializeField] private Vector3 fieldCenter; // Tâm sân
    [SerializeField] private float fieldLength; // Chiều dài sân (Z)
    [SerializeField] private float fieldWidth; // Chiều rộng sân (X)
    [SerializeField] private float fieldHeight; // Độ cao spawn (Y)
    
    [Header("Spawn Areas")]
    [SerializeField] private Vector3 player1HomeCenter; // Trung tâm sân nhà Player1
    [SerializeField] private Vector3 player2HomeCenter; // Trung tâm sân nhà Player2
    [SerializeField] private Vector3 centerLine; // Đường giữa sân
    
    void Start()
    {
        DetectFieldSize();
        CalculateSpawnAreas();
    }
    
    void DetectFieldSize()
    {
        if (fieldPlane != null)
        {
            // Lấy kích thước thực tế từ plane
            Renderer renderer = fieldPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                fieldSize = renderer.bounds.size;
                fieldCenter = renderer.bounds.center;
                fieldLength = fieldSize.z;
                fieldWidth = fieldSize.x;
                fieldHeight = fieldCenter.y + (fieldSize.y * 0.5f) + 0.1f; // Trên mặt plane một chút
                
                Debug.Log($"Field detected: Size={fieldSize}, Center={fieldCenter}");
            }
        }
        
        // Nếu không có plane, tính từ goals
        if (goals != null && goals.Length >= 2)
        {
            Vector3 goal1Pos = goals[0].position;
            Vector3 goal2Pos = goals[1].position;
            
            fieldLength = Mathf.Abs(goal2Pos.z - goal1Pos.z) + 1f; // Thêm 1f cho an toàn
            fieldCenter = (goal1Pos + goal2Pos) * 0.5f;
            
            if (fieldWidth == 0) fieldWidth = 4f; // Default width nếu không detect được
            
            Debug.Log($"Field calculated from goals: Length={fieldLength}, Center={fieldCenter}");
        }
    }
    
    void CalculateSpawnAreas()
    {
        // Tính toán các khu vực spawn
        centerLine = new Vector3(fieldCenter.x, fieldHeight, fieldCenter.z);
        
        // Sân nhà Player1 (Z âm - gần goal có Z nhỏ hơn)
        player1HomeCenter = new Vector3(fieldCenter.x, fieldHeight, fieldCenter.z - fieldLength * 0.25f);
        
        // Sân nhà Player2 (Z dương - gần goal có Z lớn hơn)
        player2HomeCenter = new Vector3(fieldCenter.x, fieldHeight, fieldCenter.z + fieldLength * 0.25f);
        
        Debug.Log($"Player1 Home: {player1HomeCenter}, Player2 Home: {player2HomeCenter}");
    }
    
    // API để các script khác sử dụng
    public Vector3 GetFieldSize() => fieldSize;
    public Vector3 GetFieldCenter() => fieldCenter;
    public float GetFieldLength() => fieldLength;
    public float GetFieldWidth() => fieldWidth;
    public float GetFieldHeight() => fieldHeight;
    
    // Spawn paddle tại trung tâm sân nhà
    public Vector3 GetHomeSpawn(bool isPlayer1)
    {
        return isPlayer1 ? player1HomeCenter : player2HomeCenter;
    }
    
    // Spawn paddle random trong sân nhà
    public Vector3 GetRandomHomeSpawn(bool isPlayer1)
    {
        Vector3 homeCenter = isPlayer1 ? player1HomeCenter : player2HomeCenter;
        
        // Random trong phạm vi 60% kích thước sân nhà
        float randomX = Random.Range(-fieldWidth * 0.3f, fieldWidth * 0.3f);
        float randomZ = Random.Range(-fieldLength * 0.15f, fieldLength * 0.15f);
        
        return new Vector3(homeCenter.x + randomX, fieldHeight, homeCenter.z + randomZ);
    }
    
    // Spawn puck tại trung tâm sân
    public Vector3 GetPuckCenterSpawn()
    {
        return centerLine;
    }
    
    // Spawn puck gần sân nhà của team thua
    public Vector3 GetPuckLoserSpawn(bool loserIsPlayer1)
    {
        Vector3 basePos = loserIsPlayer1 ? player1HomeCenter : player2HomeCenter;
        
        // Random nhẹ xung quanh trung tâm sân nhà
        float randomX = Random.Range(-fieldWidth * 0.2f, fieldWidth * 0.2f);
        float randomZ = Random.Range(-fieldLength * 0.1f, fieldLength * 0.1f);
        
        return new Vector3(basePos.x + randomX, fieldHeight, basePos.z + randomZ);
    }
    
    // Kiểm tra vị trí có trong giới hạn sân không
    public bool IsInFieldBounds(Vector3 position)
    {
        float halfWidth = fieldWidth * 0.5f;
        float halfLength = fieldLength * 0.5f;
        
        return (Mathf.Abs(position.x - fieldCenter.x) <= halfWidth) &&
               (Mathf.Abs(position.z - fieldCenter.z) <= halfLength);
    }
    
    // Clamp position vào trong sân
    public Vector3 ClampToField(Vector3 position)
    {
        float halfWidth = fieldWidth * 0.5f;
        float halfLength = fieldLength * 0.5f;
        
        position.x = Mathf.Clamp(position.x, fieldCenter.x - halfWidth, fieldCenter.x + halfWidth);
        position.z = Mathf.Clamp(position.z, fieldCenter.z - halfLength, fieldCenter.z + halfLength);
        position.y = fieldHeight;
        
        return position;
    }
    
    // Clamp paddle vào sân nhà của nó
    public Vector3 ClampToHomeField(Vector3 position, bool isPlayer1)
    {
        float halfWidth = fieldWidth * 0.5f;
        
        position.x = Mathf.Clamp(position.x, fieldCenter.x - halfWidth, fieldCenter.x + halfWidth);
        position.y = fieldHeight;
        
        if (isPlayer1)
        {
            // Player1 chỉ được ở nửa sân Z âm
            position.z = Mathf.Clamp(position.z, fieldCenter.z - fieldLength * 0.5f, fieldCenter.z);
        }
        else
        {
            // Player2 chỉ được ở nửa sân Z dương
            position.z = Mathf.Clamp(position.z, fieldCenter.z, fieldCenter.z + fieldLength * 0.5f);
        }
        
        return position;
    }
    
    void OnDrawGizmosSelected()
    {
        if (fieldSize == Vector3.zero) return;
        
        // Vẽ toàn bộ sân
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(fieldCenter, fieldSize);
        
        // Vẽ đường giữa sân
        Gizmos.color = Color.yellow;
        Vector3 lineStart = new Vector3(fieldCenter.x - fieldWidth * 0.5f, fieldHeight, fieldCenter.z);
        Vector3 lineEnd = new Vector3(fieldCenter.x + fieldWidth * 0.5f, fieldHeight, fieldCenter.z);
        Gizmos.DrawLine(lineStart, lineEnd);
        
        // Vẽ sân nhà Player1 (xanh)
        Gizmos.color = Color.blue;
        Vector3 p1Size = new Vector3(fieldWidth * 0.8f, 0.1f, fieldLength * 0.4f);
        Gizmos.DrawWireCube(player1HomeCenter, p1Size);
        
        // Vẽ sân nhà Player2 (đỏ)
        Gizmos.color = Color.red;
        Vector3 p2Size = new Vector3(fieldWidth * 0.8f, 0.1f, fieldLength * 0.4f);
        Gizmos.DrawWireCube(player2HomeCenter, p2Size);
        
        // Vẽ spawn points
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(player1HomeCenter, 0.1f);
        Gizmos.DrawSphere(player2HomeCenter, 0.1f);
        Gizmos.DrawSphere(centerLine, 0.1f);
    }
}

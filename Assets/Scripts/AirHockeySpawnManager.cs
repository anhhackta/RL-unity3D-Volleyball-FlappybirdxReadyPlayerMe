using UnityEngine;

public class AirHockeySpawnManager : MonoBehaviour
{
    [Header("Plane sân đấu")]
    public Transform fieldPlane; // Kéo plane sân vào đây
    
    [Header("Kích thước sân (Auto-detect từ plane)")]
    [SerializeField] private float fieldX; // Chiều dài sân (X) 
    [SerializeField] private float fieldZ; // Chiều rộng sân (Z)
    [SerializeField] private Vector3 fieldCenter; // Tâm sân
    [SerializeField] private float fieldHeight; // Độ cao spawn
    
    [Header("Phân chia sân")]
    [SerializeField] private float player1AreaMinX; // Phần sân Player1: X min
    [SerializeField] private float player1AreaMaxX; // Phần sân Player1: X max  
    [SerializeField] private float player2AreaMinX; // Phần sân Player2: X min
    [SerializeField] private float player2AreaMaxX; // Phần sân Player2: X max
    
    void Start()
    {
        DetectFieldSize();
        CalculatePlayerAreas();
    }
    
    void DetectFieldSize()
    {
        if (fieldPlane != null)
        {
            // Lấy kích thước thực tế từ plane
            Renderer renderer = fieldPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector3 size = renderer.bounds.size;
                fieldX = size.x; // Chiều dài sân
                fieldZ = size.z; // Chiều rộng sân
                fieldCenter = renderer.bounds.center;
                fieldHeight = fieldCenter.y + (size.y * 0.5f) + 0.1f; // Trên mặt plane
                
                Debug.Log($"Field detected - X (length): {fieldX}, Z (width): {fieldZ}, Center: {fieldCenter}");
            }
        }
        else
        {
            Debug.LogWarning("Field Plane chưa được gán! Sử dụng default values");
            fieldX = 4f;
            fieldZ = 4f;
            fieldCenter = Vector3.zero;
            fieldHeight = 0.5f;
        }
    }
    
    void CalculatePlayerAreas()
    {
        // Chia chiều dài (X) làm 2 phần cho mỗi paddle
        float halfX = fieldX * 0.5f;
        
        // Player1 ở phần X âm (bên trái)
        player1AreaMinX = fieldCenter.x - halfX;
        player1AreaMaxX = fieldCenter.x;
        
        // Player2 ở phần X dương (bên phải)  
        player2AreaMinX = fieldCenter.x;
        player2AreaMaxX = fieldCenter.x + halfX;
        
        Debug.Log($"Player1 area X: {player1AreaMinX} to {player1AreaMaxX}");
        Debug.Log($"Player2 area X: {player2AreaMinX} to {player2AreaMaxX}");
    }
    
    // Lấy vị trí sân nhà của paddle (trung tâm phần sân)
    public Vector3 GetHomeFieldCenter(bool isPlayer1)
    {
        float homeX = isPlayer1 ? (player1AreaMinX + player1AreaMaxX) * 0.5f : (player2AreaMinX + player2AreaMaxX) * 0.5f;
        return new Vector3(homeX, fieldHeight, fieldCenter.z);
    }
    
    // Lấy vị trí spawn ngẫu nhiên trong sân nhà của paddle
    public Vector3 GetRandomHomeSpawn(bool isPlayer1)
    {
        float randomX, randomZ;
        
        if (isPlayer1)
        {
            // Player1: phần X âm (bên trái)
            randomX = Random.Range(player1AreaMinX + 0.2f, player1AreaMaxX - 0.2f);
        }
        else
        {
            // Player2: phần X dương (bên phải)
            randomX = Random.Range(player2AreaMinX + 0.2f, player2AreaMaxX - 0.2f);
        }
        
        // Random Z trong toàn bộ chiều rộng sân (trừ biên)
        float halfZ = fieldZ * 0.5f;
        randomZ = Random.Range(fieldCenter.z - halfZ + 0.2f, fieldCenter.z + halfZ - 0.2f);
        
        return new Vector3(randomX, fieldHeight, randomZ);
    }
    
    // Lấy vị trí spawn puck ở giữa sân
    public Vector3 GetPuckCenterSpawn()
    {
        return new Vector3(fieldCenter.x, fieldHeight, fieldCenter.z);
    }
    
    // Lấy vị trí spawn puck gần sân nhà của team thua
    public Vector3 GetPuckLoserSideSpawn(bool loserIsPlayer1)
    {
        float spawnX = loserIsPlayer1 ? 
            Random.Range(player1AreaMinX + 0.3f, player1AreaMaxX - 0.3f) :
            Random.Range(player2AreaMinX + 0.3f, player2AreaMaxX - 0.3f);
            
        float randomZ = Random.Range(fieldCenter.z - fieldZ * 0.3f, fieldCenter.z + fieldZ * 0.3f);
        
        return new Vector3(spawnX, fieldHeight, randomZ);
    }
    
    // Kiểm tra vị trí có hợp lệ trong sân nhà của paddle không
    public bool IsValidPosition(Vector3 position, bool isPlayer1)
    {
        // Kiểm tra Z boundary (chiều rộng sân)
        float halfZ = fieldZ * 0.5f;
        if (Mathf.Abs(position.z - fieldCenter.z) > halfZ) return false;
        
        // Kiểm tra X boundary theo sân nhà
        if (isPlayer1)
        {
            return position.x >= player1AreaMinX && position.x <= player1AreaMaxX;
        }
        else
        {
            return position.x >= player2AreaMinX && position.x <= player2AreaMaxX;
        }
    }
    
    // Clamp paddle vào sân nhà của nó
    public Vector3 ClampToHomeArea(Vector3 position, bool isPlayer1)
    {
        // Clamp Z theo chiều rộng sân
        float halfZ = fieldZ * 0.5f;
        position.z = Mathf.Clamp(position.z, fieldCenter.z - halfZ, fieldCenter.z + halfZ);
        position.y = fieldHeight;
        
        // Clamp X theo sân nhà
        if (isPlayer1)
        {
            position.x = Mathf.Clamp(position.x, player1AreaMinX, player1AreaMaxX);
        }
        else
        {
            position.x = Mathf.Clamp(position.x, player2AreaMinX, player2AreaMaxX);
        }
        
        return position;
    }
    
    // Getter methods
    public float GetFieldX() => fieldX;
    public float GetFieldZ() => fieldZ;
    public Vector3 GetFieldCenter() => fieldCenter;
    public float GetFieldHeight() => fieldHeight;
    
    void OnDrawGizmosSelected()
    {
        if (fieldX == 0 || fieldZ == 0) return;
        
        // Vẽ toàn bộ sân (trắng)
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(fieldCenter, new Vector3(fieldX, 0.1f, fieldZ));
        
        // Vẽ đường chia giữa sân (vàng)
        Gizmos.color = Color.yellow;
        Vector3 lineStart = new Vector3(fieldCenter.x, fieldHeight, fieldCenter.z - fieldZ * 0.5f);
        Vector3 lineEnd = new Vector3(fieldCenter.x, fieldHeight, fieldCenter.z + fieldZ * 0.5f);
        Gizmos.DrawLine(lineStart, lineEnd);
        
        // Vẽ sân nhà Player1 (xanh - bên trái)
        Gizmos.color = Color.blue;
        Vector3 p1Center = new Vector3((player1AreaMinX + player1AreaMaxX) * 0.5f, fieldHeight, fieldCenter.z);
        Vector3 p1Size = new Vector3(fieldX * 0.5f, 0.1f, fieldZ * 0.8f);
        Gizmos.DrawWireCube(p1Center, p1Size);
        
        // Vẽ sân nhà Player2 (đỏ - bên phải)
        Gizmos.color = Color.red;
        Vector3 p2Center = new Vector3((player2AreaMinX + player2AreaMaxX) * 0.5f, fieldHeight, fieldCenter.z);
        Vector3 p2Size = new Vector3(fieldX * 0.5f, 0.1f, fieldZ * 0.8f);
        Gizmos.DrawWireCube(p2Center, p2Size);
        
        // Vẽ spawn points
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetHomeFieldCenter(true), 0.1f); // Player1 spawn
        Gizmos.DrawSphere(GetHomeFieldCenter(false), 0.1f); // Player2 spawn
        Gizmos.DrawSphere(GetPuckCenterSpawn(), 0.1f); // Puck center spawn
    }
}

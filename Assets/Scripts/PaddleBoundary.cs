using UnityEngine;

public class PaddleBoundary : MonoBehaviour
{
    [Header("Boundary Settings")]
    public bool isPlayer1 = true; // True cho paddle 1, False cho paddle 2
    public float minX = -2f;
    public float maxX = 2f;
    public float fixedY = 0.5f; // Độ cao cố định
    
    [Header("Z Boundaries")]
    public float player1MinZ = -2f;
    public float player1MaxZ = -0.1f;
    public float player2MinZ = 0.1f;
    public float player2MaxZ = 2f;
    
    void LateUpdate()
    {
        // Giữ paddle trong giới hạn
        Vector3 pos = transform.position;
        
        // Giới hạn X
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        
        // Giữ Y cố định
        pos.y = fixedY;
        
        // Giới hạn Z dựa trên player
        if (isPlayer1)
        {
            pos.z = Mathf.Clamp(pos.z, player1MinZ, player1MaxZ);
        }
        else
        {
            pos.z = Mathf.Clamp(pos.z, player2MinZ, player2MaxZ);
        }
        
        transform.position = pos;
    }
}

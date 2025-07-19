using UnityEngine;
using GameAI.Player;
using GameAI.AI;

namespace GameAI.Environment
{
    public class ArenaSetup : MonoBehaviour
    {
        public Transform pointStart, pointPlayer1, pointPlayer2, pointPuck1, pointPuck2;
        public Transform player1, player2, puck;
        
        [Header("Auto Reset")]
        public bool autoResetOnStart = false; // Tắt auto reset để tránh conflict

        public void ResetPositions(int lastGoalPlayer)
        {
            Debug.Log($"ArenaSetup: ResetPositions called with lastGoalPlayer={lastGoalPlayer}");
            
            // Kiểm tra Point references
            if (pointPlayer1 == null || pointPlayer2 == null || pointStart == null)
            {
                Debug.LogError("ArenaSetup: Missing Point references!");
                return;
            }
            
            // Reset vị trí paddle
            if (player1 != null)
            {
                Debug.Log($"Moving Player1 from {player1.position} to {pointPlayer1.position}");
                player1.position = pointPlayer1.position;
            }
            if (player2 != null)
            {
                Debug.Log($"Moving Player2 from {player2.position} to {pointPlayer2.position}");
                player2.position = pointPlayer2.position;
            }
            
            // Reset rigidbody velocity của paddle
            Rigidbody p1Rb = player1?.GetComponent<Rigidbody>();
            Rigidbody p2Rb = player2?.GetComponent<Rigidbody>();
            if (p1Rb != null)
            {
                p1Rb.linearVelocity = Vector3.zero;
                p1Rb.angularVelocity = Vector3.zero;
            }
            if (p2Rb != null)
            {
                p2Rb.linearVelocity = Vector3.zero;
                p2Rb.angularVelocity = Vector3.zero;
            }
            
            // Reset vị trí puck
            if (puck != null)
            {
                Vector3 puckTargetPos;
                if (lastGoalPlayer == -1)
                    puckTargetPos = pointStart.position;
                else if (lastGoalPlayer == 0)
                    puckTargetPos = pointPuck1.position;
                else
                    puckTargetPos = pointPuck2.position;
                
                Debug.Log($"Moving Puck from {puck.position} to {puckTargetPos}");
                puck.position = puckTargetPos;
            }

            // Reset velocity puck
            Rigidbody puckRb = puck?.GetComponent<Rigidbody>();
            if (puckRb != null)
            {
                puckRb.linearVelocity = Vector3.zero;
                puckRb.angularVelocity = Vector3.zero;
            }
        }

        void Start()
        {
            // Chỉ reset khi được bật (để tránh conflict với vị trí ban đầu)
            if (autoResetOnStart)
            {
                ResetPositions(-1); // Start game
            }
            else
            {
                Debug.Log("ArenaSetup: Auto reset disabled - keeping initial positions");
            }
        }
    }
}

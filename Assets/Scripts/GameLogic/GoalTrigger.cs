using UnityEngine;
using GameAI.AI;

namespace GameAI.GameLogic
{
    public class GoalTrigger : MonoBehaviour
    {
        public int playerIndex; // 0: Player1, 1: Player2 (player thua bàn)
        public GameManager gameManager;
        
        [Header("AI Training")]
        public PaddleAgent agent1; // Agent Player 1
        public PaddleAgent agent2; // Agent Player 2

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Puck"))
            {
                gameManager.GoalScored(playerIndex);
                
                // Thông báo cho cả 2 AI về kết quả
                if (agent1 != null && agent2 != null)
                {
                    if (playerIndex == 0) // Player 1 thua, Player 2 thắng
                    {
                        agent1.OnGoalScored(false); // Player 1 thua
                        agent2.OnGoalScored(true);  // Player 2 thắng
                    }
                    else // Player 2 thua, Player 1 thắng
                    {
                        agent1.OnGoalScored(true);  // Player 1 thắng
                        agent2.OnGoalScored(false); // Player 2 thua
                    }
                }
            }
        }
    }
}
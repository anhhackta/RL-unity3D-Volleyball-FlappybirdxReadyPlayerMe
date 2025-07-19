using UnityEngine;
using GameAI.Environment;

namespace GameAI.GameLogic
{
    public class GameManager : MonoBehaviour
    {
        public ScoreManager scoreManager;
        public ArenaSetup arenaSetup;
        public PuckController puckController;
        
        [Header("Training Mode")]
        public bool isTrainingMode = true;

        public void GoalScored(int loserIndex)
        {
            // Cộng điểm cho đội thắng (đội không phải là loser)
            int winnerIndex = loserIndex == 0 ? 1 : 0;
            if (scoreManager != null)
            {
                scoreManager.AddScore(winnerIndex);
            }
            
            // Trong training mode, không cần reset ngay (AI sẽ tự reset qua EndEpisode)
            if (!isTrainingMode)
            {
                ResetGamePositions(loserIndex);
            }
            
            // Chỉ log khi không phải training mode
            if (!isTrainingMode)
            {
                Debug.Log($"Goal! Player {winnerIndex + 1} scored! Score: {scoreManager.player1Score} - {scoreManager.player2Score}");
            }
        }

        public void ResetGame()
        {
            // Reset toàn bộ game (dành cho training)
            if (scoreManager != null && !isTrainingMode)
            {
                scoreManager.ResetScore();
            }
            
            ResetGamePositions(-1); // Reset về PointStart
        }

        private void ResetGamePositions(int lastGoalPlayer)
        {
            // Reset vị trí paddle và puck về đúng điểm
            if (arenaSetup != null)
            {
                arenaSetup.ResetPositions(lastGoalPlayer);
            }
            
            // Đảm bảo puck được reset hoàn toàn
            if (puckController != null)
            {
                puckController.ResetPuck(lastGoalPlayer);
            }
        }

        // Dành cho training - reset đơn giản hơn
        public void ResetForTraining()
        {
            if (arenaSetup != null)
            {
                arenaSetup.ResetPositions(-1);
            }
        }
    }
}
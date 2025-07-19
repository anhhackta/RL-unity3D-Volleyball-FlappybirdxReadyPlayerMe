using UnityEngine;

namespace GameAI.GameLogic
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Score Tracking")]
        public int player1Score = 0;
        public int player2Score = 0;
        
        [Header("Training Stats")]
        public bool isTrainingMode = true;
        public int totalGoals = 0;
        public int player1Wins = 0;
        public int player2Wins = 0;

        public void AddScore(int playerIndex)
        {
            if (playerIndex == 0) 
            {
                player1Score++;
                if (isTrainingMode) player1Wins++;
            }
            else 
            {
                player2Score++;
                if (isTrainingMode) player2Wins++;
            }
            
            if (isTrainingMode) totalGoals++;
            
            // Chỉ log khi cần thiết (training không cần log nhiều)
            if (!isTrainingMode)
            {
                Debug.Log($"Score: {player1Score} - {player2Score}");
            }
        }

        public void ResetScore()
        {
            player1Score = 0;
            player2Score = 0;
        }

        // Reset toàn bộ stats (cho training session mới)
        public void ResetTrainingStats()
        {
            ResetScore();
            totalGoals = 0;
            player1Wins = 0;
            player2Wins = 0;
        }

        // In stats training (gọi định kỳ)
        public void LogTrainingStats()
        {
            if (isTrainingMode && totalGoals > 0)
            {
                float winRate1 = (float)player1Wins / totalGoals * 100;
                float winRate2 = (float)player2Wins / totalGoals * 100;
                Debug.Log($"Training Stats - Total Goals: {totalGoals}, P1 Win Rate: {winRate1:F1}%, P2 Win Rate: {winRate2:F1}%");
            }
        }
    }
}
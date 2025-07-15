using UnityEngine;
using TMPro;

public class ScoreboardController : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshPro scoreText; // Text Mesh Pro component trên cube
    
    [Header("Scoreboard Settings")]
    public string defaultText = "Air Hockey\n0 : 0";
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;
    public Color defaultColor = Color.white;
    
    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    
    void Start()
    {
        // Thiết lập text mặc định
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshPro>();
        }
        
        ResetScoreboard();
    }
    
    public void UpdateScore(int player1Score, int player2Score)
    {
        scorePlayer1 = player1Score;
        scorePlayer2 = player2Score;
        
        // Cập nhật text
        string displayText = $"Air Hockey\n{scorePlayer1} : {scorePlayer2}";
        
        if (scoreText != null)
        {
            scoreText.text = displayText;
            
            // Đổi màu text dựa trên điểm số
            if (scorePlayer1 > scorePlayer2)
            {
                scoreText.color = player1Color;
            }
            else if (scorePlayer2 > scorePlayer1)
            {
                scoreText.color = player2Color;
            }
            else
            {
                scoreText.color = defaultColor;
            }
        }
    }
    
    public void ResetScoreboard()
    {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        
        if (scoreText != null)
        {
            scoreText.text = defaultText;
            scoreText.color = defaultColor;
        }
    }
    
    public void ShowGameOver(bool player1Won)
    {
        string winnerText = player1Won ? "Player 1 Wins!" : "Player 2 Wins!";
        string displayText = $"{winnerText}\n{scorePlayer1} : {scorePlayer2}";
        
        if (scoreText != null)
        {
            scoreText.text = displayText;
            scoreText.color = player1Won ? player1Color : player2Color;
        }
    }
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

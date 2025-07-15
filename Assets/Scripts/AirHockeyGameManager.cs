using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AirHockeyGameManager : MonoBehaviour
{
    [Header("Game Objects")]
    public Transform puck;
    public Transform goal1; // Gôn của Paddle 2 (Z dương)
    public Transform goal2; // Gôn của Paddle 1 (Z âm)
    
    [Header("UI Text Components")]
    public TextMeshProUGUI gameScoreText; // TextMeshPro UI trong Game Canvas (Camera2 - Player vs AI)
    
    [Header("3D Scoreboard")]
    public ScoreboardController scoreboardCube; // Cube với TextMesh (luôn cập nhật)
    
    [Header("AI Agents")]
    public AirHockeyAgent agent1; // Tham chiếu đến agent1
    public AirHockeyAgent agent2; // Tham chiếu đến agent2
    
    private Rigidbody puckRb;
    private int scorePlayer1 = 0; // Điểm của Paddle 1 (AI hoặc người chơi)
    private int scorePlayer2 = 0; // Điểm của Paddle 2 (AI)
    private Vector3 puckStartPos = new Vector3(0, 0.5f, 0);
    
    public int maxScore = 5; // Điểm tối đa để kết thúc game
    private bool gameActive = false; // Trạng thái game đang chạy

    void Start()
    {
        // Kiểm tra và lấy Rigidbody của puck
        if (puck != null)
        {
            puckRb = puck.GetComponent<Rigidbody>();
            if (puckRb == null)
            {
                Debug.LogError($"Puck '{puck.name}' thiếu component Rigidbody! Hãy thêm Rigidbody vào puck.");
                return;
            }
        }
        else
        {
            Debug.LogError("Puck chưa được gán trong AirHockeyGameManager!");
            return;
        }
        
        ResetPuck(true);
        UpdateScoreText();
    }

    public void OnGoalScored(bool isGoal1)
    {
        if (!gameActive) return; // Không xử lý nếu game đã dừng
        
        if (isGoal1) // Puck vào gôn 1 (Paddle 2 thua)
        {
            scorePlayer1++;
            ResetPuck(false); // Đặt puck bên Paddle 2
            
            // Thông báo cho agents
            if (agent1 != null) agent1.OnGoalScored(false); // Agent1 ghi bàn
            if (agent2 != null) agent2.OnGoalScored(true);  // Agent2 bị ghi bàn
        }
        else // Puck vào gôn 2 (Paddle 1 thua)
        {
            scorePlayer2++;
            ResetPuck(true); // Đặt puck bên Paddle 1
            
            // Thông báo cho agents
            if (agent1 != null) agent1.OnGoalScored(true);  // Agent1 bị ghi bàn
            if (agent2 != null) agent2.OnGoalScored(false); // Agent2 ghi bàn
        }
        UpdateScoreText();
        
        // Kiểm tra kết thúc game
        if (scorePlayer1 >= maxScore || scorePlayer2 >= maxScore)
        {
            EndGame();
        }
    }

    void ResetPuck(bool towardsPlayer1)
    {
        if (puckRb == null) return; // Bảo vệ khỏi lỗi null
        
        puckRb.linearVelocity = Vector3.zero;
        puckRb.angularVelocity = Vector3.zero;
        
        // Đặt puck gần phía team thua (spawn random trong sân nhà của team thua)
        Vector3 puckSpawnPos;
        if (towardsPlayer1) // Player1 thua
        {
            float randomX = Random.Range(-0.8f, 0.8f);
            float spawnZ = Random.Range(-0.8f, -0.3f); // Gần Player1 (Z âm)
            puckSpawnPos = new Vector3(randomX, 0.5f, spawnZ);
        }
        else // Player2 thua
        {
            float randomX = Random.Range(-0.8f, 0.8f);
            float spawnZ = Random.Range(0.3f, 0.8f); // Gần Player2 (Z dương)
            puckSpawnPos = new Vector3(randomX, 0.5f, spawnZ);
        }
        
        puck.position = puckSpawnPos;
        Debug.Log($"Puck spawned at: {puckSpawnPos} (towards Player{(towardsPlayer1 ? "1" : "2")})");
    }

    void UpdateScoreText()
    {
        string scoreDisplay = $"Player 1: {scorePlayer1} | Player 2: {scorePlayer2}";
        
        // Cập nhật Game Score Text (hiện trên Camera2 khi Player vs AI)
        if (gameScoreText != null)
            gameScoreText.text = scoreDisplay;
            
        // Cập nhật Scoreboard Cube (luôn cập nhật cho cả 2 chế độ)
        if (scoreboardCube != null)
            scoreboardCube.UpdateScore(scorePlayer1, scorePlayer2);
    }
    
    void EndGame()
    {
        gameActive = false;
        string winner = scorePlayer1 > scorePlayer2 ? "Player 1" : "Player 2";
        bool player1Won = scorePlayer1 > scorePlayer2;
        
        Debug.Log($"Game Over! {winner} Wins!");
        
        // Hiển thị kết quả trên scoreboard cube
        if (scoreboardCube != null)
            scoreboardCube.ShowGameOver(player1Won);
        
        // Reset sau 3 giây
        Invoke("ResetGame", 3f);
    }
    
    public void ResetGame()
    {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        ResetPuck(true);
        UpdateScoreText();
        
        // Reset scoreboard cube
        if (scoreboardCube != null)
            scoreboardCube.ResetScoreboard();
        
        // Reset agents nếu có và game đang chạy
        if (gameActive)
        {
            if (agent1 != null && agent1.enabled) agent1.OnEpisodeBegin();
            if (agent2 != null && agent2.enabled) agent2.OnEpisodeBegin();
        }
    }

    public void StartGame()
    {
        gameActive = true;
        ResetGame();
        Debug.Log("Game Started!");
    }
    
    public void StopGame()
    {
        gameActive = false;
        
        // Dừng puck
        if (puckRb != null)
        {
            puckRb.linearVelocity = Vector3.zero;
            puckRb.angularVelocity = Vector3.zero;
        }
        
        // Dừng agents
        if (agent1 != null && agent1.enabled) agent1.EndEpisode();
        if (agent2 != null && agent2.enabled) agent2.EndEpisode();
        
        Debug.Log("Game Stopped!");
    }
    
    public bool IsGameActive()
    {
        return gameActive;
    }
}
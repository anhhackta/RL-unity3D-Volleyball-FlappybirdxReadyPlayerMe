using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Samples.QuickStart;

public class GameModeController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera playerCamera; // Camera thế giới mở (Ready Player Me camera)
    public Camera gameCamera; // Camera zoom sát bàn game
    
    [Header("Player (Ready Player Me)")]
    public GameObject playerObject; // Player GameObject có Ready Player Me
    // LÚU Ý: Không cần kéo ThirdPersonController vào đây
    // Vì nó đã có sẵn trong playerObject rồi
    
    [Header("Air Hockey Game")]
    public GameObject airHockeyTable; // Bàn air hockey
    public AirHockeyPaddle paddle1; // Paddle của player (phía gần player)
    public AirHockeyPaddle paddle2; // Paddle của AI (phía xa)
    public AirHockeyAgent agent1; // Agent cho paddle 1 (không dùng trong Player vs AI)
    public AirHockeyAgent agent2; // Agent cho paddle 2
    
    [Header("UI Panels")]
    public GameObject worldUI; // UI thế giới mở (gồm panel RPM + chatbox AI)
    public GameObject gameUI; // UI trong mini game Air Hockey
    public GameObject gameModePanel; // Panel chọn chế độ chơi (Player vs AI / AI vs AI)
    
    [Header("Ready Player Me UI (cần ẩn khi chơi game)")]
    public GameObject rpmPanel; // Panel đổi giao diện Ready Player Me
    public GameObject chatboxPanel; // Panel chatbox AI
    
    [Header("Scoreboard")]
    public ScoreboardController scoreboardCube; // Bảng điểm Cube 3D (luôn cập nhật)
    
    [Header("Game Settings")]
    public AirHockeyGameManager gameManager;
    
    public enum GameMode
    {
        WorldExploration,
        PlayerVsAI,
        AIVsAI
    }
    
    private GameMode currentMode = GameMode.WorldExploration;
    
    void Start()
    {
        SetGameMode(GameMode.WorldExploration);
    }
    
    public void OnPlayerVsAIButtonClick()
    {
        SetGameMode(GameMode.PlayerVsAI);
    }
    
    public void OnAIVsAIButtonClick()
    {
        SetGameMode(GameMode.AIVsAI);
    }
    
    public void OnBackToWorldButtonClick()
    {
        // Dừng game trước khi chuyển về thế giới mở
        if (gameManager != null)
        {
            gameManager.StopGame();
        }
        
        SetGameMode(GameMode.WorldExploration);
    }
    
    void SetGameMode(GameMode mode)
    {
        currentMode = mode;
        
        switch (mode)
        {
            case GameMode.WorldExploration:
                SetWorldExplorationMode();
                break;
            case GameMode.PlayerVsAI:
                SetPlayerVsAIMode();
                break;
            case GameMode.AIVsAI:
                SetAIVsAIMode();
                break;
        }
    }
    
    void SetWorldExplorationMode()
    {
        // Camera settings - dùng camera Ready Player Me
        playerCamera.enabled = true;
        gameCamera.enabled = false;
        
        // Player controller - cho phép di chuyển tự do
        if (playerObject != null) 
        {
            playerObject.SetActive(true);
            var controller = playerObject.GetComponent<ThirdPersonController>();
            if (controller != null) controller.enabled = true;
        }
        
        // UI settings - hiện lại Ready Player Me UI
        if (worldUI != null) worldUI.SetActive(true);
        if (rpmPanel != null) rpmPanel.SetActive(true);
        if (chatboxPanel != null) chatboxPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (gameModePanel != null) gameModePanel.SetActive(false);
        
        // KHÔNG tắt sân đấu ở chế độ thế giới mở - chỉ tắt game logic
        // airHockeyTable.SetActive(false); // BỎ DÒNG NÀY
        
        // ScoreboardCube luôn hiển thị trong thế giới mở để xem điểm
        if (scoreboardCube != null) scoreboardCube.SetActive(true);
        
        // Disable agents
        if (agent1 != null) agent1.enabled = false;
        if (agent2 != null) agent2.enabled = false;
        
        // Disable paddles
        if (paddle1 != null) paddle1.enabled = false;
        if (paddle2 != null) paddle2.enabled = false;
    }
    
    void SetPlayerVsAIMode()
    {
        // Camera settings - chuyển sang camera zoom sát bàn
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }
        if (gameCamera != null)
        {
            gameCamera.enabled = true;
            gameCamera.gameObject.SetActive(true);
            Debug.Log("Game Camera activated for Player vs AI mode");
        }
        
        // Player controller - tắt HOÀN TOÀN di chuyển nhân vật
        if (playerObject != null)
        {
            // Tắt ThirdPersonController
            var controller = playerObject.GetComponent<ThirdPersonController>();
            if (controller != null) controller.enabled = false;
            
            // Tắt ThirdPersonMovement (nếu có)
            var movement = playerObject.GetComponent<ThirdPersonMovement>();
            if (movement != null) movement.enabled = false;
            
            // Tắt CharacterController để không thể di chuyển
            var charController = playerObject.GetComponent<CharacterController>();
            if (charController != null) charController.enabled = false;
            
            Debug.Log("Player movement completely disabled for Player vs AI mode");
        }
        
        // UI settings - ẩn Ready Player Me UI, hiện Game UI
        if (worldUI != null) worldUI.SetActive(false);
        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (chatboxPanel != null) chatboxPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        if (gameModePanel != null) gameModePanel.SetActive(false);
        
        // Enable game
        airHockeyTable.SetActive(true);
        
        // ScoreboardCube vẫn hiển thị (cập nhật điểm song song với Game UI)
        if (scoreboardCube != null) scoreboardCube.SetActive(true);
        
        // Set paddle modes - Player điều khiển paddle1, AI điều khiển paddle2
        paddle1.isPlayer1 = true;
        paddle1.enabled = true; // Player control
        paddle2.isPlayer1 = false;
        paddle2.enabled = false; // AI control
        
        // Agent settings - chỉ AI paddle2 hoạt động
        agent1.enabled = false; // Player paddle
        agent2.enabled = true; // AI paddle
        agent2.isPlayer1 = false;
        
        // Reset và start game
        gameManager.ResetGame();
        gameManager.StartGame();
    }
    
    void SetAIVsAIMode()
    {
        // Camera settings - giữ camera thế giới mở để xem từ xa
        playerCamera.enabled = true;
        gameCamera.enabled = false;
        
        // Player controller - cho phép di chuyển để xem từ xa
        if (playerObject != null) 
        {
            playerObject.SetActive(true);
            var controller = playerObject.GetComponent<ThirdPersonController>();
            if (controller != null) controller.enabled = true;
        }
        
        // UI settings - giữ Ready Player Me UI nhưng ẩn panels không cần thiết
        if (worldUI != null) worldUI.SetActive(true);
        if (rpmPanel != null) rpmPanel.SetActive(false); // Ẩn để tập trung xem game
        if (chatboxPanel != null) chatboxPanel.SetActive(false); // Ẩn để tập trung xem game
        if (gameUI != null) gameUI.SetActive(false);
        if (gameModePanel != null) gameModePanel.SetActive(false);
        
        // Enable game
        airHockeyTable.SetActive(true);
        
        // ScoreboardCube hiển thị để xem điểm từ xa
        if (scoreboardCube != null) scoreboardCube.SetActive(true);
        
        // Disable player paddles - chỉ AI điều khiển
        paddle1.enabled = false;
        paddle2.enabled = false;
        
        // Enable both agents
        agent1.enabled = true;
        agent1.isPlayer1 = true;
        agent2.enabled = true;
        agent2.isPlayer1 = false;
        
        // Reset và start game
        gameManager.ResetGame();
        gameManager.StartGame();
    }
    
    public void ShowGameModePanel()
    {
        gameModePanel.SetActive(true);
    }
    
    public void HideGameModePanel()
    {
        gameModePanel.SetActive(false);
    }
    
    public GameMode GetCurrentMode()
    {
        return currentMode;
    }
}

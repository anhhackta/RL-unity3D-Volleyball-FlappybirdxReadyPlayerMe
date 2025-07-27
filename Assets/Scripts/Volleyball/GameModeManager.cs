using UnityEngine;
using Unity.MLAgents.Policies;

public enum GameMode
{
    AIvsAI,
    AIvsPlayer
}

public class GameModeManager : MonoBehaviour
{
    [Header("Game Mode Settings")]
    public GameMode currentGameMode = GameMode.AIvsAI;
    public bool showGameModeOnStart = true;
    
    [Header("Agents")]
    public VolleyballAgent blueAgent;
    public VolleyballAgent purpleAgent;
    
    [Header("UI")]
    public GameObject gameModePanel; // Panel có sẵn của bạn
    public UnityEngine.UI.Button aiVsAiButton;
    public UnityEngine.UI.Button aiVsPlayerButton;
    public UnityEngine.UI.Button closeButton;
    
    [Header("Input Settings")]
    public KeyCode playerMoveUp = KeyCode.W;
    public KeyCode playerMoveDown = KeyCode.S;
    public KeyCode playerMoveLeft = KeyCode.A;
    public KeyCode playerMoveRight = KeyCode.D;
    public KeyCode playerJump = KeyCode.Space;
    
    private BehaviorParameters blueAgentBehavior;
    private BehaviorParameters purpleAgentBehavior;
    
    void Start()
    {
        Debug.Log("🚀 GameModeManager Start() được gọi");
        
        // Get behavior parameters
        if (blueAgent != null)
            blueAgentBehavior = blueAgent.GetComponent<BehaviorParameters>();
        if (purpleAgent != null)
            purpleAgentBehavior = purpleAgent.GetComponent<BehaviorParameters>();
        
        Debug.Log($"Blue Agent: {(blueAgent != null ? "✅" : "❌")}, Purple Agent: {(purpleAgent != null ? "✅" : "❌")}");
        Debug.Log($"Game Mode Panel: {(gameModePanel != null ? "✅" : "❌")}");
        
        // Setup button events
        if (aiVsAiButton != null)
            aiVsAiButton.onClick.AddListener(() => SetGameMode(GameMode.AIvsAI));
        if (aiVsPlayerButton != null)
            aiVsPlayerButton.onClick.AddListener(() => SetGameMode(GameMode.AIvsPlayer));
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseGameModePanel);
        
        Debug.Log($"Buttons - AI vs AI: {(aiVsAiButton != null ? "✅" : "❌")}, AI vs Player: {(aiVsPlayerButton != null ? "✅" : "❌")}, Close: {(closeButton != null ? "✅" : "❌")}");
        
        // Show game mode selection on start if enabled
        if (showGameModeOnStart)
        {
            Debug.Log("⏸️ Pause game và chuẩn bị hiện panel...");
            // Pause game immediately
            Time.timeScale = 0f;
            
            // Show game mode panel immediately instead of using Invoke
            ShowGameModePanel();
        }
        else
        {
            Debug.Log("➡️ Không hiện panel, chạy mode mặc định");
            // Set initial game mode normally
            SetGameMode(currentGameMode);
        }
    }
    
    void Update()
    {
        // Handle player input when in AI vs Player mode
        if (currentGameMode == GameMode.AIvsPlayer)
        {
            HandlePlayerInput();
        }
    }
    
    public void ShowGameModePanel()
    {
        Debug.Log("🎮 ShowGameModePanel() được gọi");
        
        if (gameModePanel != null)
        {
            Debug.Log("✅ gameModePanel tìm thấy, đang hiện panel...");
            
            // Kiểm tra Canvas
            Canvas canvas = gameModePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"Canvas tìm thấy: {canvas.name}, enabled: {canvas.enabled}");
            }
            else
            {
                Debug.LogError("❌ Không tìm thấy Canvas parent!");
            }
            
            // Kiểm tra parent objects
            Transform parent = gameModePanel.transform.parent;
            while (parent != null)
            {
                Debug.Log($"Parent: {parent.name}, active: {parent.gameObject.activeInHierarchy}");
                parent = parent.parent;
            }
            
            gameModePanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log($"Panel active: {gameModePanel.activeInHierarchy}, Time scale: {Time.timeScale}");
        }
        else
        {
            Debug.LogError("❌ gameModePanel is NULL! Hãy gán panel vào GameModeManager");
        }
    }
    
    public void CloseGameModePanel()
    {
        Debug.Log("🔒 CloseGameModePanel() được gọi");
        
        if (gameModePanel != null)
        {
            Debug.Log("✅ Đang đóng panel...");
            gameModePanel.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log($"Panel active: {gameModePanel.activeInHierarchy}, Time scale: {Time.timeScale}");
        }
        else
        {
            Debug.LogError("❌ gameModePanel is NULL!");
        }
    }
    
    public void SetGameMode(GameMode mode)
    {
        Debug.Log($"🎮 SetGameMode({mode}) được gọi");
        currentGameMode = mode;
        
        switch (mode)
        {
            case GameMode.AIvsAI:
                Debug.Log("🤖 Setup AI vs AI mode");
                SetupAIvsAI();
                break;
            case GameMode.AIvsPlayer:
                Debug.Log("👤 Setup AI vs Player mode");
                SetupAIvsPlayer();
                break;
        }
        
        CloseGameModePanel();
        
        // Resume game after mode selection
        Time.timeScale = 1f;
        
        Debug.Log($"✅ Game mode changed to: {mode}, Time scale: {Time.timeScale}");
    }
    
    void SetupAIvsAI()
    {
        // Enable AI for both agents
        if (blueAgentBehavior != null)
            blueAgentBehavior.BehaviorType = BehaviorType.Default;
        if (purpleAgentBehavior != null)
            purpleAgentBehavior.BehaviorType = BehaviorType.Default;
        
        // Enable both agents
        if (blueAgent != null)
            blueAgent.gameObject.SetActive(true);
        if (purpleAgent != null)
            purpleAgent.gameObject.SetActive(true);
    }
    
    void SetupAIvsPlayer()
    {
        // Blue agent stays AI
        if (blueAgentBehavior != null)
            blueAgentBehavior.BehaviorType = BehaviorType.Default;
        
        // Purple agent becomes player controlled
        if (purpleAgentBehavior != null)
            purpleAgentBehavior.BehaviorType = BehaviorType.HeuristicOnly;
        
        // Enable both agents
        if (blueAgent != null)
            blueAgent.gameObject.SetActive(true);
        if (purpleAgent != null)
            purpleAgent.gameObject.SetActive(true);
    }
    
    void HandlePlayerInput()
    {
        // This will use the Heuristic method in VolleyballAgent
        // The input handling is already implemented in VolleyballAgent.Heuristic()
        // We can customize the key bindings here if needed
        
        // Display controls info
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Player Controls:\n" +
                     "WASD - Move and Rotate\n" +
                     "Arrow Keys - Move\n" +
                     "Space - Jump\n" +
                     "H - Show this help");
        }
    }
    
    // Public methods for UI buttons
    public void OnAIvsAIButton()
    {
        SetGameMode(GameMode.AIvsAI);
    }
    
    public void OnAIvsPlayerButton()
    {
        SetGameMode(GameMode.AIvsPlayer);
    }
}

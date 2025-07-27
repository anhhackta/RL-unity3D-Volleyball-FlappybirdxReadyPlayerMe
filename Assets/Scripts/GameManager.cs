using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("References")]
    public Bird bird;
    public UIManager uiManager;
    
    public bool isAIMode = false;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        ShowMenu();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu();
        }
    }
    
    public void SetGameMode(bool aiMode)
    {
        isAIMode = aiMode;
        bird.SetGameMode(aiMode);
    }
    
    public void StartGame()
    {
        // Enable all movement
        Ground[] grounds = FindObjectsByType<Ground>(FindObjectsSortMode.None);
        foreach (Ground ground in grounds)
        {
            ground.enabled = true;
        }
        
        PipeBehavior[] pipes = FindObjectsByType<PipeBehavior>(FindObjectsSortMode.None);
        foreach (PipeBehavior pipe in pipes)
        {
            pipe.enabled = true;
        }
        
        bird.StartGame();
        uiManager.ShowGameUI();
    }
    
    public void GameOver()
    {
        // Stop bird
        bird.StopGame();
        
        // Stop ground movement
        Ground[] grounds = FindObjectsByType<Ground>(FindObjectsSortMode.None);
        foreach (Ground ground in grounds)
        {
            ground.enabled = false;
        }
        
        // Stop pipe movement
        PipeBehavior[] pipes = FindObjectsByType<PipeBehavior>(FindObjectsSortMode.None);
        foreach (PipeBehavior pipe in pipes)
        {
            pipe.enabled = false;
        }
        
        uiManager.ShowGameOver();
    }
    
    public void RestartGame()
    {
        bird.RestartGame();
        StartGame();
    }
    
    public void ShowMenu()
    {
        // Stop all game movement
        bird.StopGame();
        
        // Stop ground movement
        Ground[] grounds = FindObjectsByType<Ground>(FindObjectsSortMode.None);
        foreach (Ground ground in grounds)
        {
            ground.enabled = false;
        }
        
        // Stop pipe movement
        PipeBehavior[] pipes = FindObjectsByType<PipeBehavior>(FindObjectsSortMode.None);
        foreach (PipeBehavior pipe in pipes)
        {
            pipe.enabled = false;
        }
        
        uiManager.ShowMenu();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    
    [Header("Buttons")]
    public Button playerModeButton;
    public Button aiModeButton;
    public Button restartButton;
    public Button menuButton;
    public Button homeButton;
    
    [Header("Text")]
    public Text scoreText;
    public Text finalScoreText;
    
    private void Start()
    {
        if (playerModeButton != null) playerModeButton.onClick.AddListener(() => StartGame(false));
        if (aiModeButton != null) aiModeButton.onClick.AddListener(() => StartGame(true));
        if (restartButton != null) restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        if (menuButton != null) menuButton.onClick.AddListener(() => GameManager.Instance.ShowMenu());
        if (homeButton != null) homeButton.onClick.AddListener(() => GameManager.Instance.ShowMenu());
    }
    
    private void StartGame(bool aiMode)
    {
        GameManager.Instance.SetGameMode(aiMode);
        GameManager.Instance.StartGame();
    }
    
    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }
    
    public void ShowGameUI()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }
    
    public void ShowGameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        
        if (finalScoreText != null && GameManager.Instance.bird != null)
        {
            int finalScore = Mathf.FloorToInt(GameManager.Instance.bird.counter / 2f);
            finalScoreText.text = "Score: " + finalScore.ToString();
        }
    }
    
    private void Update()
    {
        // Update score only during gameplay
        if (scoreText != null && GameManager.Instance.bird != null && 
            gamePanel.activeInHierarchy && !menuPanel.activeInHierarchy)
        {
            int currentScore = Mathf.FloorToInt(GameManager.Instance.bird.counter / 2f);
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Game Mode Panel")]
    public GameObject gameModePanel;
    public Button playerVsAIButton;
    public Button aiVsAIButton;
    public Button backButton;
    
    [Header("Game UI (Canvas của Camera2)")]
    public GameObject gameUI; // Canvas của Camera2 (chỉ có Exit Button)
    public Button exitGameButton;
    
    [Header("World UI (UI thế giới mở)")]
    public GameObject worldUI; // UI thế giới mở (RPM + Chatbox)
    
    public GameModeController gameModeController;
    
    void Start()
    {
        // Gán events cho buttons
        if (playerVsAIButton != null)
            playerVsAIButton.onClick.AddListener(() => gameModeController.OnPlayerVsAIButtonClick());
            
        if (aiVsAIButton != null)
            aiVsAIButton.onClick.AddListener(() => gameModeController.OnAIVsAIButtonClick());
            
        if (backButton != null)
            backButton.onClick.AddListener(() => gameModeController.OnBackToWorldButtonClick());
            
        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(() => OnExitGameButtonClick());
    }
    
    public void UpdateScore(string scoreText)
    {
        // Không cần cập nhật Game Score Text nữa, chỉ dùng ScoreboardCube
        // ScoreboardCube tự cập nhật thông qua AirHockeyGameManager
    }
    
    public void ShowGameModePanel()
    {
        if (gameModePanel != null)
            gameModePanel.SetActive(true);
    }
    
    public void HideGameModePanel()
    {
        if (gameModePanel != null)
            gameModePanel.SetActive(false);
    }
    
    public void ShowGameUI()
    {
        if (gameUI != null)
            gameUI.SetActive(true);
    }
    
    public void HideGameUI()
    {
        if (gameUI != null)
            gameUI.SetActive(false);
    }
    
    public void ShowWorldUI()
    {
        if (worldUI != null)
            worldUI.SetActive(true);
    }
    
    public void HideWorldUI()
    {
        if (worldUI != null)
            worldUI.SetActive(false);
    }

    public void OnExitGameButtonClick()
    {
        if (gameModeController != null)
        {
            gameModeController.OnBackToWorldButtonClick();
        }
    }
}

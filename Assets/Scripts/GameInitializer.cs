using UnityEngine;
using ReadyPlayerMe.Samples.QuickStart;

public class GameInitializer : MonoBehaviour
{
    [Header("Game Controllers")]
    public GameModeController gameModeController;
    public UIManager uiManager;
    public AirHockeyGameManager gameManager;
    
    [Header("Player")]
    public GameObject playerObject; // Player GameObject Ready Player Me
    // LÚU Ý: Không cần kéo ThirdPersonController vào đây
    
    [Header("Cameras")]
    public Camera playerCamera;
    public Camera gameCamera;
    
    void Start()
    {
        // Thiết lập ban đầu
        InitializeGame();
    }
    
    void InitializeGame()
    {
        // Đảm bảo bắt đầu ở chế độ thế giới mở
        if (playerCamera != null) playerCamera.enabled = true;
        if (gameCamera != null) gameCamera.enabled = false;
        
        // Kích hoạt điều khiển nhân vật
        if (playerObject != null) 
        {
            playerObject.SetActive(true);
            var controller = playerObject.GetComponent<ThirdPersonController>();
            if (controller != null) controller.enabled = true;
        }
        
        // Thiết lập UI ban đầu
        if (uiManager != null)
        {
            uiManager.ShowWorldUI();
            uiManager.HideGameUI();
            uiManager.HideGameModePanel();
        }
        
        Debug.Log("Game Initialized - World Exploration Mode");
    }
    
    public void ResetToWorldMode()
    {
        if (gameModeController != null)
        {
            gameModeController.OnBackToWorldButtonClick();
        }
    }
}

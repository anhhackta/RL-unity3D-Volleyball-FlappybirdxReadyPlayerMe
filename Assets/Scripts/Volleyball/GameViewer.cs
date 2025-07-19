using UnityEngine;
using UnityEngine.SceneManagement;

public class GameViewer : MonoBehaviour
{
    [Header("Camera Control")]
    public Camera[] cameras;
    public KeyCode cameraSwitchKey = KeyCode.R;
    
    [Header("Game Control")]
    public KeyCode pauseKey = KeyCode.P;
    public KeyCode exitKey = KeyCode.Escape;
    public string mainMenuSceneName = "MainMenu";
    
    [Header("UI")]
    public GameObject pauseMenuUI;
    public GameObject gameUI;
    
    private int currentCameraIndex = 0;
    private bool isPaused = false;
    
    void Start()
    {
        // Setup camera
        if (cameras.Length > 0)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].gameObject.SetActive(i == 0);
            }
        }
        
        // Hide pause menu
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }
    
    void Update()
    {
        // Camera switch
        if (Input.GetKeyDown(cameraSwitchKey))
        {
            SwitchCamera();
        }
        
        // Pause/Resume
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
        
        // Exit
        if (Input.GetKeyDown(exitKey))
        {
            ExitToMainMenu();
        }
    }
    
    void SwitchCamera()
    {
        if (cameras.Length <= 1) return;
        
        cameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
        cameras[currentCameraIndex].gameObject.SetActive(true);
        
        Debug.Log($"Đã chuyển sang camera {currentCameraIndex + 1}");
    }
    
    void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
            
        if (gameUI != null)
            gameUI.SetActive(false);
            
        Debug.Log("Game đã tạm dừng");
    }
    
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
            
        if (gameUI != null)
            gameUI.SetActive(true);
            
        Debug.Log("Game đã tiếp tục");
    }
    
    void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
        Debug.Log("Đã thoát về menu chính");
    }
    
    // Public methods for UI buttons
    public void OnResumeButton()
    {
        ResumeGame();
    }
    
    public void OnExitButton()
    {
        ExitToMainMenu();
    }
    
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 
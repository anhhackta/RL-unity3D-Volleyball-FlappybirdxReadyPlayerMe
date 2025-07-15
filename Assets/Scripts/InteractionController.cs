/*using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public GameObject pressFText; 
    public GameObject infoPanel;
    public GameObject panelBlocker;
    public GameModeController gameModeController; // Tham chiếu đến GameModeController

    private bool isInTrigger = false;

    void Start()
    {
        // Đảm bảo mọi thứ đều được ẩn khi bắt đầu
        if (pressFText != null) pressFText.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (panelBlocker != null) panelBlocker.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressFText.SetActive(true);
            isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressFText.SetActive(false);
            isInTrigger = false;
            CloseInfoPanel(); // Gọi hàm đóng để đảm bảo cả panel và blocker đều tắt
        }
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            // Nếu panel chưa mở thì mở panel chọn chế độ
            if (!infoPanel.activeSelf)
            {
                infoPanel.SetActive(true);
                panelBlocker.SetActive(true);
                
                // Hiển thị panel chọn chế độ game
                if (gameModeController != null)
                {
                    gameModeController.ShowGameModePanel();
                }
            }
            else
            {
                CloseInfoPanel();
            }
        }
    }

    // Hàm này được gọi bởi nút Đóng
    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
        panelBlocker.SetActive(false);
        
        // Ẩn panel chọn chế độ game
        if (gameModeController != null)
        {
            gameModeController.HideGameModePanel();
        }
    }
}*/
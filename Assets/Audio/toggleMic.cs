using UnityEngine;
using UnityEngine.UI;

public class MicrophoneToggle : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private Sprite microphoneSprite;  // Gán microphone.png vào đây
    [SerializeField] private Sprite muteSprite;       // Gán mute.png vào đây
    
    [Header("Component References")]
    [SerializeField] private Image buttonImage;
    
    private bool isMuted = false; // Mặc định ban đầu là unmute

    private void Start()
    {
        // Lấy component Image nếu chưa gán
        if (buttonImage == null) buttonImage = GetComponent<Image>();
        
        // Cập nhật hình ảnh ban đầu
        UpdateButtonImage();
        
        // Thêm sự kiện click
        GetComponent<Button>().onClick.AddListener(ToggleMicrophoneState);
    }

    private void ToggleMicrophoneState()
    {
        // Đổi trạng thái
        isMuted = !isMuted;
        
        // Cập nhật hình ảnh
        UpdateButtonImage();
        
        // Thêm logic xử lý mute/unmute thực tế ở đây
        if (isMuted)
        {
            Debug.Log("Microphone muted");
            // Thêm code mute microphone
        }
        else
        {
            Debug.Log("Microphone unmuted");
            // Thêm code unmute microphone
        }
    }

    private void UpdateButtonImage()
    {
        buttonImage.sprite = isMuted ? muteSprite : microphoneSprite;
    }
}
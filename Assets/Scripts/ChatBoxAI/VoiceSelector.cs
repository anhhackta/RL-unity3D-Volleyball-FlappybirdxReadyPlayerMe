using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoiceSelector : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown voiceDropdown;
    public GeminiChatbot chatbot;
    public Button ttsButton; // Button thay thế toggle TTS
    
    [Header("TTS Button Settings")]
    public Sprite ttsOnSprite;  // Mic bật
    public Sprite ttsOffSprite; // Mic tắt
    
    private bool isTtsEnabled = true;
    private Image ttsButtonImage;
    
    void Start()
    {
        SetupVoiceDropdown();
        SetupTtsButton();
            
        if (voiceDropdown != null)
            voiceDropdown.onValueChanged.AddListener(OnVoiceChanged);
    }
    
    void SetupTtsButton()
    {
        if (ttsButton != null)
        {
            ttsButtonImage = ttsButton.GetComponent<Image>();
            ttsButton.onClick.AddListener(ToggleTts);
            UpdateTtsButtonSprite();
        }
    }
    
    void SetupVoiceDropdown()
    {
        if (voiceDropdown == null) return;
        
        voiceDropdown.ClearOptions();
        
        // Thêm tất cả các option voice
        var voiceNames = new System.Collections.Generic.List<string>
        {
            "MCViet (Nam, Việt Nam)",
            "VietDung (Nam, Việt Nam)", 
            "Adam (Nam, Mỹ)",
            "Bella (Nữ, Mỹ)",
            "Antoni (Nam, Ba Lan)",
            "Elli (Nữ, Mỹ)",
            "Josh (Nam, Mỹ)",
            "Arnold (Nam, Mỹ)",
            "Sam (Nam, Mỹ)",
            "BUPPIX (Giọng đặc biệt)"
        };
        
        voiceDropdown.AddOptions(voiceNames);
        
        // Đặt giá trị mặc định là BUPPIX (index 9)
        voiceDropdown.value = 9; // BUPPIX
        voiceDropdown.RefreshShownValue();
    }
    
    public void OnVoiceChanged(int voiceIndex)
    {
        if (chatbot != null)
        {
            chatbot.ChangeVoice(voiceIndex);
        }
    }
    
    public void ToggleTts()
    {
        isTtsEnabled = !isTtsEnabled;
        UpdateTtsButtonSprite();
        
        // Cập nhật TTS state trong chatbot
        if (chatbot != null)
        {
            chatbot.SetTtsEnabled(isTtsEnabled);
        }
        
        Debug.Log($"TTS {(isTtsEnabled ? "ON" : "OFF")}");
    }
    
    private void UpdateTtsButtonSprite()
    {
        if (ttsButtonImage != null)
        {
            ttsButtonImage.sprite = isTtsEnabled ? ttsOnSprite : ttsOffSprite;
        }
    }
    
    public bool IsTtsEnabled()
    {
        return isTtsEnabled;
    }
    
    public void SetTtsState(bool state)
    {
        isTtsEnabled = state;
        UpdateTtsButtonSprite();
        
        if (chatbot != null)
        {
            chatbot.SetTtsEnabled(isTtsEnabled);
        }
    }
}

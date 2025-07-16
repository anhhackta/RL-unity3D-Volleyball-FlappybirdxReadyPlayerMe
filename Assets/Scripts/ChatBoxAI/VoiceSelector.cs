using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoiceSelector : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown voiceDropdown;
    public GeminiChatbot chatbot;
    
    void Start()
    {
        SetupVoiceDropdown();
            
        if (voiceDropdown != null)
            voiceDropdown.onValueChanged.AddListener(OnVoiceChanged);
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
}

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class SimpleChatbotManager : MonoBehaviour
{
    [Header("🔑 API Configuration")]
    public TextAsset apiKeysJson;
    
    [Header("🎤 UI Components")]
    public TMP_InputField inputField;
    public TMP_Text chatDisplayText;
    public Button sendButton;
    public Button ttsButton;
    public Button toggleChatButton;
    public ScrollRect chatScrollRect;
    public TMP_Dropdown voiceDropdown;
    public GameObject chatContent;
    public RectTransform chatPanel;
    
    [Header("🎨 Button Sprites")]
    public Sprite ttsOnSprite;
    public Sprite ttsOffSprite;
    public Sprite chatShowSprite;
    public Sprite chatHideSprite;
    
    [Header("⚙️ Settings")]
    [TextArea(3, 5)]
    public string systemPrompt = "Bạn là trợ lý AI thông minh. Trả lời ngắn gọn bằng tiếng Việt một cách thân thiện và hữu ích.";
    public int maxResponseLength = 80;
    public string responseStyle = "thân thiện và súc tích";
    public ElevenLabsTTS.VoiceID selectedVoice = ElevenLabsTTS.VoiceID.BUPPIX;
    public int maxChatHistory = 10;
    public bool ttsEnabled = true;
    public bool preventMovementWhenTyping = true;
    
    // Private variables
    private string geminiApiKey = "";
    private string elevenLabsApiKey = "";
    private string geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
    
    private List<ChatMessage> chatHistory = new List<ChatMessage>();
    private ChatbotTextContent[] geminiChatHistory;
    private AudioSource audioSource;
    private bool isProcessing = false;
    
    // UI State
    private bool isChatVisible = false; // Bắt đầu với chatbox ẩn
    private bool isInputFocused = false;
    private static bool globalMovementBlocked = false;
    
    // Component references
    private Image ttsButtonImage;
    private Image toggleChatButtonImage;

    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    private void Initialize()
    {
        // Load API keys
        LoadApiKeys();
        
        // Setup audio
        SetupAudio();
        
        // Setup UI
        SetupUI();
        
        // Setup voice dropdown
        SetupVoiceDropdown();
        
        // Initialize chat
        geminiChatHistory = new ChatbotTextContent[] { };
        AddMessageToChat("AI Assistant", "Xin chào! Tôi là trợ lý AI của bạn. Tôi có thể giúp gì cho bạn?", true);
        
        // Hide entire chatbox initially
        if (chatPanel != null)
            chatPanel.gameObject.SetActive(false);
    }
    
    private void LoadApiKeys()
    {
        if (apiKeysJson != null)
        {
            ApiKeys keys = JsonUtility.FromJson<ApiKeys>(apiKeysJson.text);
            geminiApiKey = keys.geminiKey;
            elevenLabsApiKey = keys.elevenLabsKey;
            Debug.Log($"API Keys loaded: Gemini={!string.IsNullOrEmpty(geminiApiKey)}, ElevenLabs={!string.IsNullOrEmpty(elevenLabsApiKey)}");
        }
        else
        {
            Debug.LogError("API Keys JSON file not assigned!");
        }
    }
    
    private void SetupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    private void SetupUI()
    {
        // Setup buttons
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);
            
        if (ttsButton != null)
        {
            ttsButtonImage = ttsButton.GetComponent<Image>();
            ttsButton.onClick.AddListener(ToggleTts);
            UpdateTtsButtonSprite();
        }
        
        if (toggleChatButton != null)
        {
            toggleChatButtonImage = toggleChatButton.GetComponent<Image>();
            toggleChatButton.onClick.AddListener(ToggleChat);
            UpdateToggleChatButtonSprite();
        }
        
        // Setup input field
        if (inputField != null)
        {
            inputField.onSubmit.AddListener((text) => SendMessage());
            inputField.onSelect.AddListener(OnInputFieldSelected);
            inputField.onDeselect.AddListener(OnInputFieldDeselected);
        }
    }
    
    private void SetupVoiceDropdown()
    {
        if (voiceDropdown == null) return;
        
        voiceDropdown.ClearOptions();
        var voiceNames = new List<string>
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
        voiceDropdown.value = 9; // BUPPIX default
        voiceDropdown.RefreshShownValue();
        voiceDropdown.onValueChanged.AddListener(OnVoiceChanged);
    }
    
    private void HandleInput()
    {
        // Check input field focus
        isInputFocused = inputField != null && inputField.isFocused;
        globalMovementBlocked = preventMovementWhenTyping && isInputFocused;
        
        // Handle Enter to send message
        if (isInputFocused && Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(inputField.text.Trim()))
                SendMessage();
        }
    }
    
    public void SendMessage()
    {
        if (isProcessing || string.IsNullOrEmpty(inputField.text.Trim()))
            return;
            
        string userMessage = inputField.text.Trim();
        inputField.text = "";
        
        AddMessageToChat("You", userMessage, false);
        StartCoroutine(SendChatRequestToGemini(userMessage));
    }
    
    private IEnumerator SendChatRequestToGemini(string newMessage)
    {
        isProcessing = true;
        if (sendButton != null) sendButton.interactable = false;

        if (string.IsNullOrEmpty(geminiApiKey))
        {
            Debug.LogError("Gemini API key is not set!");
            AddMessageToChat("System", "API key not configured.", true);
            isProcessing = false;
            if (sendButton != null) sendButton.interactable = true;
            yield break;
        }

        string url = $"{geminiEndpoint}?key={geminiApiKey}";
        
        // Build system instruction
        string systemInstruction = $"{systemPrompt}\nStyle: {responseStyle}\nKeep responses under {maxResponseLength} words.";
        
        ChatbotTextContent userContent = new ChatbotTextContent
        {
            role = "user",
            parts = new ChatbotTextPart[] { new ChatbotTextPart { text = newMessage } }
        };

        ChatbotTextContent instruction = new ChatbotTextContent
        {
            parts = new ChatbotTextPart[] { new ChatbotTextPart { text = systemInstruction } }
        };

        List<ChatbotTextContent> contentsList = new List<ChatbotTextContent>(geminiChatHistory);
        contentsList.Add(userContent);
        geminiChatHistory = contentsList.ToArray();

        ChatbotChatRequest chatRequest = new ChatbotChatRequest 
        { 
            contents = geminiChatHistory, 
            system_instruction = instruction 
        };

        string jsonData = JsonUtility.ToJson(chatRequest);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError("Gemini API Error: " + www.error);
                AddMessageToChat("System", "Lỗi kết nối AI service.", true);
            } 
            else 
            {
                ChatbotTextResponse response = JsonUtility.FromJson<ChatbotTextResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                {
                    string reply = response.candidates[0].content.parts[0].text;
                    
                    // Add bot response to history
                    ChatbotTextContent botContent = new ChatbotTextContent
                    {
                        role = "model",
                        parts = new ChatbotTextPart[] { new ChatbotTextPart { text = reply } }
                    };
                    
                    contentsList.Add(botContent);
                    geminiChatHistory = contentsList.ToArray();
                    
                    // Limit history
                    if (geminiChatHistory.Length > maxChatHistory * 2)
                    {
                        ChatbotTextContent[] newHistory = new ChatbotTextContent[maxChatHistory * 2];
                        Array.Copy(geminiChatHistory, geminiChatHistory.Length - (maxChatHistory * 2), newHistory, 0, maxChatHistory * 2);
                        geminiChatHistory = newHistory;
                    }
                    
                    AddMessageToChat("AI Assistant", reply, true);
                    
                    // Play TTS if enabled
                    if (ttsEnabled)
                    {
                        StartCoroutine(PlayTextToSpeech(reply));
                    }
                }
                else
                {
                    AddMessageToChat("System", "Không nhận được phản hồi từ AI.", true);
                }
            }
        }
        
        isProcessing = false;
        if (sendButton != null) sendButton.interactable = true;
    }
    
    private IEnumerator PlayTextToSpeech(string text)
    {
        if (string.IsNullOrEmpty(elevenLabsApiKey))
        {
            Debug.LogWarning("ElevenLabs API key not set!");
            yield break;
        }
        
        yield return StartCoroutine(ElevenLabsTTS.GetTextToSpeechAudio(text, elevenLabsApiKey, selectedVoice, (audioClip) =>
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }));
    }
    
    private void AddMessageToChat(string sender, string message, bool isBot)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            role = sender,
            message = message,
            timestamp = DateTime.Now
        };
        
        chatHistory.Add(chatMessage);
        
        if (chatHistory.Count > maxChatHistory)
            chatHistory.RemoveAt(0);
        
        UpdateChatDisplay();
    }
    
    private void UpdateChatDisplay()
    {
        if (chatDisplayText == null) return;
        
        string displayText = "";
        foreach (ChatMessage msg in chatHistory)
        {
            string timeStr = msg.timestamp.ToString("HH:mm");
            string color = msg.role == "You" ? "#4CAF50" : "#2196F3";
            displayText += $"<color={color}><b>[{timeStr}] {msg.role}:</b></color>\n{msg.message}\n\n";
        }
        
        chatDisplayText.text = displayText;
        
        if (chatScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    // UI Event Handlers
    public void ToggleTts()
    {
        ttsEnabled = !ttsEnabled;
        UpdateTtsButtonSprite();
        Debug.Log($"TTS {(ttsEnabled ? "ON" : "OFF")}");
    }
    
    public void ToggleChat()
    {
        isChatVisible = !isChatVisible;
        if (chatPanel != null)
            chatPanel.gameObject.SetActive(isChatVisible);
        UpdateToggleChatButtonSprite();
    }
    
    public void OnVoiceChanged(int voiceIndex)
    {
        if (voiceIndex >= 0 && voiceIndex < System.Enum.GetValues(typeof(ElevenLabsTTS.VoiceID)).Length)
        {
            selectedVoice = (ElevenLabsTTS.VoiceID)voiceIndex;
            Debug.Log($"Voice changed to: {selectedVoice}");
        }
    }
    
    private void OnInputFieldSelected(string text)
    {
        isInputFocused = true;
        Debug.Log("Chat input focused - Movement blocked");
    }
    
    private void OnInputFieldDeselected(string text)
    {
        isInputFocused = false;
        Debug.Log("Chat input unfocused - Movement enabled");
    }
    
    private void UpdateTtsButtonSprite()
    {
        if (ttsButtonImage != null)
            ttsButtonImage.sprite = ttsEnabled ? ttsOnSprite : ttsOffSprite;
    }
    
    private void UpdateToggleChatButtonSprite()
    {
        if (toggleChatButtonImage != null)
            toggleChatButtonImage.sprite = isChatVisible ? chatHideSprite : chatShowSprite;
    }
    
    // Static method for character movement scripts
    public static bool IsMovementBlocked()
    {
        return globalMovementBlocked;
    }
    
    // Public getters
    public bool IsInputFocused() => isInputFocused;
    public bool IsTtsEnabled() => ttsEnabled;
    public bool IsChatVisible() => isChatVisible;
}

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class ApiKeys
{
    public string geminiKey;
    public string elevenLabsKey;
}

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string message;
    public DateTime timestamp;
}

[System.Serializable]
public class ChatbotTextPart
{
    public string text;
}

[System.Serializable]
public class ChatbotTextContent
{
    public string role;
    public ChatbotTextPart[] parts;
}

[System.Serializable]
public class ChatbotTextCandidate
{
    public ChatbotTextContent content;
}

[System.Serializable]
public class ChatbotTextResponse
{
    public ChatbotTextCandidate[] candidates;
}

[System.Serializable]
public class ChatbotChatRequest
{
    public ChatbotTextContent[] contents;
    public ChatbotTextContent system_instruction;
}

public class GeminiChatbot : MonoBehaviour
{
    [Header("API Configuration")]
    public TextAsset apiKeysJson;
    
    [Header("UI Components")]
    public TMP_InputField inputField;
    public TMP_Text chatDisplayText;
    public Button sendButton;
    public ScrollRect chatScrollRect;
    
    [Header("Manager References")]
    public PromptConfigManager promptConfigManager;
    public InputFieldManager inputFieldManager;
    public ResizableChatPanel resizableChatPanel;
    
    [Header("Chat Settings")]
    public string botInstructions = "You are a helpful AI assistant. Keep responses concise and friendly.";
    public int maxChatHistory = 10;
    
    [Header("TTS Settings")]
    public ElevenLabsTTS.VoiceID selectedVoice = ElevenLabsTTS.VoiceID.BUPPIX;
    public bool ttsEnabled = true; // Thay thế toggle bằng bool
    
    // Private variables
    private string geminiApiKey = "";
    private string elevenLabsApiKey = "";
    private string geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
    private string elevenLabsEndpoint = "https://api.elevenlabs.io/v1/text-to-speech/";
    private string voiceId = "BUPPIXeDaJWBz696iXRS"; // Will be updated based on selectedVoice
    
    private List<ChatMessage> chatHistory = new List<ChatMessage>();
    private ChatbotTextContent[] geminiChatHistory;
    private AudioSource audioSource;
    private bool isProcessing = false;

    void Start()
    {
        // Load API keys
        if (apiKeysJson != null)
        {
            ApiKeys keys = JsonUtility.FromJson<ApiKeys>(apiKeysJson.text);
            geminiApiKey = keys.geminiKey;
            elevenLabsApiKey = keys.elevenLabsKey;
            
            // Debug API key loading
            Debug.Log($"Gemini API Key loaded: {(!string.IsNullOrEmpty(geminiApiKey) ? "YES" : "NO")}");
            Debug.Log($"Gemini API Key length: {geminiApiKey?.Length ?? 0}");
            if (!string.IsNullOrEmpty(geminiApiKey))
            {
                Debug.Log($"Gemini API Key starts with: {geminiApiKey.Substring(0, Math.Min(10, geminiApiKey.Length))}...");
            }
        }
        else
        {
            Debug.LogError("API Keys JSON file not assigned!");
        }
        
        // Set voice ID from selected voice
        voiceId = ElevenLabsTTS.GetVoiceIDString(selectedVoice);
        
        // Setup audio source for TTS
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Initialize Gemini chat history
        geminiChatHistory = new ChatbotTextContent[] { };
        
        // Setup UI events
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);
            
        if (inputField != null)
        {
            inputField.onSubmit.AddListener((text) => SendMessage());
        }
        
        // Setup managers if not assigned
        if (promptConfigManager == null)
            promptConfigManager = FindFirstObjectByType<PromptConfigManager>();
        if (inputFieldManager == null)
            inputFieldManager = FindFirstObjectByType<InputFieldManager>();
        if (resizableChatPanel == null)
            resizableChatPanel = FindFirstObjectByType<ResizableChatPanel>();
        
        // Subscribe to prompt config changes
        if (promptConfigManager != null)
        {
            promptConfigManager.OnPromptConfigChanged += OnPromptConfigChanged;
        }
        
        // Add welcome message
        AddMessageToChat("AI Assistant", "Hello! I'm your AI assistant. How can I help you today?", true);
    }

    public void SendMessage()
    {
        if (isProcessing || string.IsNullOrEmpty(inputField.text.Trim()))
            return;
            
        string userMessage = inputField.text.Trim();
        inputField.text = "";
        
        // Add user message to chat
        AddMessageToChat("You", userMessage, false);
        
        // Send to Gemini
        StartCoroutine(SendChatRequestToGemini(userMessage));
    }
    
    private IEnumerator SendChatRequestToGemini(string newMessage)
    {
        isProcessing = true;
        sendButton.interactable = false;

        // Validate API key
        if (string.IsNullOrEmpty(geminiApiKey))
        {
            Debug.LogError("Gemini API key is not set!");
            AddMessageToChat("System", "API key not configured. Please check api_keys.json file.", true);
            isProcessing = false;
            sendButton.interactable = true;
            yield break;
        }

        string url = $"{geminiEndpoint}?key={geminiApiKey}";
     
        ChatbotTextContent userContent = new ChatbotTextContent
        {
            role = "user",
            parts = new ChatbotTextPart[]
            {
                new ChatbotTextPart { text = newMessage }
            }
        };

        ChatbotTextContent instruction = new ChatbotTextContent
        {
            parts = new ChatbotTextPart[]
            {
                new ChatbotTextPart { text = GetSystemInstruction() }
            }
        }; 

        List<ChatbotTextContent> contentsList = new List<ChatbotTextContent>(geminiChatHistory);
        contentsList.Add(userContent);
        geminiChatHistory = contentsList.ToArray(); 

        ChatbotChatRequest chatRequest = new ChatbotChatRequest { contents = geminiChatHistory, system_instruction = instruction };

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
                Debug.LogError("Response Code: " + www.responseCode);
                Debug.LogError("Response Text: " + www.downloadHandler.text);
                Debug.LogError("Request URL: " + url);
                AddMessageToChat("System", "Sorry, there was an error connecting to the AI service.", true);
            } 
            else 
            {
                ChatbotTextResponse response = JsonUtility.FromJson<ChatbotTextResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                {
                    string reply = response.candidates[0].content.parts[0].text;
                    
                    // Add bot response to Gemini chat history
                    ChatbotTextContent botContent = new ChatbotTextContent
                    {
                        role = "model",
                        parts = new ChatbotTextPart[]
                        {
                            new ChatbotTextPart { text = reply }
                        }
                    };
                    
                    contentsList.Add(botContent);
                    geminiChatHistory = contentsList.ToArray();
                    
                    // Limit chat history
                    if (geminiChatHistory.Length > maxChatHistory * 2)
                    {
                        ChatbotTextContent[] newHistory = new ChatbotTextContent[maxChatHistory * 2];
                        Array.Copy(geminiChatHistory, geminiChatHistory.Length - (maxChatHistory * 2), newHistory, 0, maxChatHistory * 2);
                        geminiChatHistory = newHistory;
                    }
                    
                    // Add to UI chat
                    AddMessageToChat("AI Assistant", reply, true);
                    
                    // Text-to-speech if enabled
                    if (ttsEnabled)
                    {
                        StartCoroutine(PlayTextToSpeech(reply));
                    }
                }
                else
                {
                    AddMessageToChat("System", "No response received from AI.", true);
                }
            }
        }
        
        isProcessing = false;
        sendButton.interactable = true;
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
            else
            {
                Debug.LogError("Failed to get TTS audio");
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
        
        // Limit chat display history
        if (chatHistory.Count > maxChatHistory)
        {
            chatHistory.RemoveAt(0);
        }
        
        UpdateChatDisplay();
    }
    
    public void ChangeVoice(ElevenLabsTTS.VoiceID newVoice)
    {
        selectedVoice = newVoice;
        voiceId = ElevenLabsTTS.GetVoiceIDString(selectedVoice);
        Debug.Log($"Voice changed to: {selectedVoice} ({voiceId})");
    }
    
    public void ChangeVoice(int voiceIndex)
    {
        if (voiceIndex >= 0 && voiceIndex < System.Enum.GetValues(typeof(ElevenLabsTTS.VoiceID)).Length)
        {
            selectedVoice = (ElevenLabsTTS.VoiceID)voiceIndex;
            voiceId = ElevenLabsTTS.GetVoiceIDString(selectedVoice);
            Debug.Log($"Voice changed to: {selectedVoice} ({voiceId})");
        }
    }
    
    private void UpdateChatDisplay()
    {
        if (chatDisplayText == null) return;
        
        string displayText = "";
        
        foreach (ChatMessage msg in chatHistory)
        {
            string timeStr = msg.timestamp.ToString("HH:mm");
            displayText += $"<color={(msg.role == "You" ? "#4CAF50" : "#2196F3")}><b>[{timeStr}] {msg.role}:</b></color>\n";
            displayText += $"{msg.message}\n\n";
        }
        
        chatDisplayText.text = displayText;
        
        // Auto scroll to bottom
        if (chatScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    private void OnPromptConfigChanged(PromptConfig newConfig)
    {
        Debug.Log("Prompt configuration updated");
        // The system instruction will be updated automatically on next message
    }
    
    private string GetSystemInstruction()
    {
        if (promptConfigManager != null)
        {
            return promptConfigManager.BuildSystemInstruction();
        }
        return botInstructions;
    }
    
    public void SetTtsEnabled(bool enabled)
    {
        ttsEnabled = enabled;
        Debug.Log($"TTS set to: {(enabled ? "enabled" : "disabled")}");
    }
    
    public bool IsTtsEnabled()
    {
        return ttsEnabled;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (promptConfigManager != null)
        {
            promptConfigManager.OnPromptConfigChanged -= OnPromptConfigChanged;
        }
    }
}

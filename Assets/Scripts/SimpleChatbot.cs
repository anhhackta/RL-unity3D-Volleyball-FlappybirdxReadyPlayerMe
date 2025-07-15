using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SimpleChatbot : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField inputField;
    public TMP_Text chatDisplayText;
    public Button sendButton;
    public ScrollRect chatScrollRect;
    
    [Header("Chat Settings")]
    public int maxChatHistory = 10;
    
    private List<ChatMessage> chatHistory = new List<ChatMessage>();
    private bool isProcessing = false;
    
    // Simple predefined responses
    private Dictionary<string, string> responses = new Dictionary<string, string>
    {
        {"hello", "Hello! How can I help you today?"},
        {"hi", "Hi there! What would you like to know?"},
        {"how are you", "I'm doing great, thank you for asking! How are you?"},
        {"what is your name", "I'm your AI assistant. You can call me Assistant!"},
        {"help", "I'm here to help! You can ask me about various topics, and I'll do my best to provide useful information."},
        {"weather", "I don't have access to real-time weather data, but you can check your local weather service!"},
        {"time", $"The current time is {DateTime.Now:HH:mm}"},
        {"date", $"Today's date is {DateTime.Now:yyyy-MM-dd}"},
        {"thank you", "You're very welcome! Is there anything else I can help you with?"},
        {"thanks", "You're welcome! Happy to help!"},
        {"bye", "Goodbye! Have a great day!"},
        {"goodbye", "See you later! Take care!"}
    };

    void Start()
    {
        // Setup UI events
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);
            
        if (inputField != null)
        {
            inputField.onSubmit.AddListener((text) => SendMessage());
        }
        
        // Add welcome message
        AddMessageToChat("AI Assistant", "Hello! I'm your simple AI assistant. How can I help you today?", true);
    }

    public void SendMessage()
    {
        if (isProcessing || string.IsNullOrEmpty(inputField.text.Trim()))
            return;
            
        string userMessage = inputField.text.Trim();
        inputField.text = "";
        
        // Add user message to chat
        AddMessageToChat("You", userMessage, false);
        
        // Process message
        StartCoroutine(ProcessMessage(userMessage));
    }
    
    private IEnumerator ProcessMessage(string message)
    {
        isProcessing = true;
        sendButton.interactable = false;
        
        // Simulate thinking delay
        yield return new WaitForSeconds(0.5f);
        
        string response = GetResponse(message.ToLower());
        
        // Add bot response
        AddMessageToChat("AI Assistant", response, true);
        
        isProcessing = false;
        sendButton.interactable = true;
    }
    
    private string GetResponse(string input)
    {
        // Check for exact matches first
        foreach (var kvp in responses)
        {
            if (input.Contains(kvp.Key))
            {
                return kvp.Value;
            }
        }
        
        // Check for common patterns
        if (input.Contains("what") && input.Contains("your"))
        {
            return "I'm a simple AI assistant built in Unity. I can answer basic questions and have conversations!";
        }
        
        if (input.Contains("game") || input.Contains("unity"))
        {
            return "This is a Unity game project with an AI chatbot! Pretty cool, right?";
        }
        
        if (input.Contains("can you") || input.Contains("do you"))
        {
            return "I can have simple conversations and answer basic questions. My capabilities are limited but I'm here to help!";
        }
        
        if (input.Contains("?"))
        {
            return "That's an interesting question! I'm a simple chatbot, so my knowledge is limited, but I'm happy to chat with you.";
        }
        
        // Default responses
        string[] defaultResponses = {
            "That's interesting! Tell me more.",
            "I see. Can you elaborate on that?",
            "Thanks for sharing! What else would you like to talk about?",
            "I understand. Is there anything specific I can help you with?",
            "That's a good point. What do you think about it?",
            "I appreciate you telling me that. What else is on your mind?"
        };
        
        return defaultResponses[UnityEngine.Random.Range(0, defaultResponses.Length)];
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
}

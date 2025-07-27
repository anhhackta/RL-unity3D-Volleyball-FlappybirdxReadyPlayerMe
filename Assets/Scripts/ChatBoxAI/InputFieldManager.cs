using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InputFieldManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField chatInputField;
    public Button sendButton;
    public GeminiChatbot chatbot;
    
    [Header("Input Settings")]
    public bool preventMovementInput = true;
    public KeyCode sendKey = KeyCode.Return;
    
    private bool isInputFieldFocused = false;
    private static bool globalInputBlocked = false; // Static để các script khác có thể check
    
    void Start()
    {
        if (chatInputField != null)
        {
            // Setup input field events
            chatInputField.onSelect.AddListener(OnInputFieldSelected);
            chatInputField.onDeselect.AddListener(OnInputFieldDeselected);
            chatInputField.onSubmit.AddListener(OnInputFieldSubmit);
        }
        
        if (sendButton != null && chatbot != null)
        {
            sendButton.onClick.AddListener(() => chatbot.SendMessage());
        }
    }
    
    void Update()
    {
        // Check if input field is currently focused
        isInputFieldFocused = chatInputField != null && chatInputField.isFocused;
        globalInputBlocked = preventMovementInput && isInputFieldFocused;
        
        // Handle Enter key to send message
        if (isInputFieldFocused && Input.GetKeyDown(sendKey))
        {
            if (!string.IsNullOrEmpty(chatInputField.text.Trim()))
            {
                if (chatbot != null)
                {
                    chatbot.SendMessage();
                }
            }
        }
    }
    
    private void OnInputFieldSelected(string text)
    {
        isInputFieldFocused = true;
        globalInputBlocked = preventMovementInput;
        Debug.Log("Chat input focused - Movement blocked");
    }
    
    private void OnInputFieldDeselected(string text)
    {
        isInputFieldFocused = false;
        globalInputBlocked = false;
        Debug.Log("Chat input unfocused - Movement enabled");
    }
    
    private void OnInputFieldSubmit(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()) && chatbot != null)
        {
            chatbot.SendMessage();
            
            // Keep focus on input field after sending
            if (chatInputField != null)
            {
                chatInputField.ActivateInputField();
            }
        }
    }
    
    // Static method để các script movement có thể check
    public static bool IsMovementBlocked()
    {
        return globalInputBlocked;
    }
    
    public bool IsInputFieldFocused()
    {
        return isInputFieldFocused;
    }
    
    public void FocusInputField()
    {
        if (chatInputField != null)
        {
            chatInputField.Select();
            chatInputField.ActivateInputField();
        }
    }
    
    public void ClearInputField()
    {
        if (chatInputField != null)
        {
            chatInputField.text = "";
        }
    }
    
    public void SetInputFieldInteractable(bool interactable)
    {
        if (chatInputField != null)
        {
            chatInputField.interactable = interactable;
        }
        
        if (sendButton != null)
        {
            sendButton.interactable = interactable;
        }
    }
}

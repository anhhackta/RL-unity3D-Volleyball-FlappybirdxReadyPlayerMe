using UnityEngine;
using System;

[System.Serializable]
public class PromptConfig
{
    [Header("Basic Settings")]
    public string systemPrompt = "Bạn là trợ lý AI thông minh. Trả lời ngắn gọn bằng tiếng Việt.";
    public int maxResponseLength = 80;
    public string language = "Vietnamese";
    
    [Header("Response Style")]
    public string responseStyle = "thân thiện và súc tích";
    public bool limitResponseLength = true;
}

public class PromptConfigManager : MonoBehaviour
{
    [Header("Prompt Configuration")]
    public PromptConfig promptConfig = new PromptConfig();
    
    [Header("JSON Configuration File")]
    public TextAsset promptConfigJson;
    
    public event Action<PromptConfig> OnPromptConfigChanged;
    
    void Start()
    {
        LoadPromptConfig();
    }
    
    public void LoadPromptConfig()
    {
        if (promptConfigJson != null)
        {
            try
            {
                PromptConfig loadedConfig = JsonUtility.FromJson<PromptConfig>(promptConfigJson.text);
                promptConfig = loadedConfig;
                Debug.Log("Prompt configuration loaded from JSON file.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load prompt config from JSON: {e.Message}. Using default config.");
            }
        }
        
        OnPromptConfigChanged?.Invoke(promptConfig);
    }
    
    public string BuildSystemInstruction()
    {
        string instruction = promptConfig.systemPrompt;
        
        if (!string.IsNullOrEmpty(promptConfig.responseStyle))
        {
            instruction += $"\n\nStyle: {promptConfig.responseStyle}";
        }
        
        if (!string.IsNullOrEmpty(promptConfig.language))
        {
            instruction += $"\nLanguage: {promptConfig.language}";
        }
        
        if (promptConfig.limitResponseLength)
        {
            instruction += $"\nKeep responses under {promptConfig.maxResponseLength} words.";
        }
        
        return instruction;
    }
    
    public void UpdatePromptConfig(PromptConfig newConfig)
    {
        promptConfig = newConfig;
        OnPromptConfigChanged?.Invoke(promptConfig);
    }
    
    public PromptConfig GetCurrentConfig()
    {
        return promptConfig;
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System;

[System.Serializable]
public class ApiConfig
{
    public string deepgram_api_key;
    public string gemini_api_key;
    public string elevenlabs_api_key;
    public string elevenlabs_voice_id;
    public string gemini_prompt;
}

public class NPCVoiceAgent : MonoBehaviour
{
    [Header("Cấu hình API")]
    public TextAsset apiConfigFile;  // Kéo file JSON vào đây
    private ApiConfig config;

    public AudioSource npcAudioSource;
    public KeyCode talkKey = KeyCode.Space;

    private bool isRecording = false;
    private string recordedFilePath;
    private AudioClip recordingClip;
    private int sampleRate = 8000;

    void Start()
    {
        LoadConfigFromTextAsset();
    }

    void Update()
    {
        if (Input.GetKeyDown(talkKey))
        {
            StartRecording();
        }
        if (Input.GetKeyUp(talkKey))
        {
            StopRecording();
        }
    }

    void LoadConfigFromTextAsset()
    {
        if (apiConfigFile == null)
        {
            Debug.LogError("Chưa gán file api_config.json vào NPCVoiceAgent!");
            return;
        }

        config = JsonUtility.FromJson<ApiConfig>(apiConfigFile.text);

        if (config == null)
            Debug.LogError("Không parse được JSON từ file!");
        else
            Debug.Log("Đã load API config thành công.");
    }

    void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Không có thiết bị micro nào!");
            return;
        }

        Debug.Log("Bắt đầu ghi âm...");
        recordingClip = Microphone.Start(null, false, 10, sampleRate);
        isRecording = true;
    }

    void StopRecording()
    {
        if (!isRecording) return;
        Microphone.End(null);
        Debug.Log("Dừng ghi âm.");
        isRecording = false;

        recordedFilePath = Path.Combine(Application.persistentDataPath, "recorded.wav");
        SaveWav(recordedFilePath, recordingClip);
        StartCoroutine(ProcessVoice(recordedFilePath));
    }

    IEnumerator ProcessVoice(string audioPath)
    {
        Debug.Log("🎤 Bắt đầu xử lý voice...");
        
        // Bước 1: Speech-to-Text
        Debug.Log("🔄 Đang gửi đến Deepgram STT...");
        string sttText = null;
        yield return SendToDeepgram(audioPath, result => sttText = result);

        if (!string.IsNullOrEmpty(sttText))
        {
            Debug.Log("✅ STT hoàn thành: " + sttText);

            // Bước 2: AI Response  
            Debug.Log("🤖 Đang gửi đến Gemini AI...");
            string aiResponse = null;
            yield return SendToGemini(sttText, result => aiResponse = result);

            if (!string.IsNullOrEmpty(aiResponse))
            {
                Debug.Log("✅ Gemini response: " + aiResponse);
                
                // Bước 3: Text-to-Speech
                Debug.Log("🔊 Đang tạo audio với ElevenLabs...");
                yield return SendToElevenLabs(aiResponse);
            }
            else
            {
                Debug.LogError("❌ Gemini không trả về response!");
            }
        }
        else
        {
            Debug.LogError("❌ STT không nhận diện được text!");
        }
        
        Debug.Log("🎯 Hoàn thành xử lý voice!");
    }

    IEnumerator SendToDeepgram(string audioPath, Action<string> callback)
    {
        Debug.Log("📡 Bắt đầu gửi audio đến Deepgram...");
        // Thêm language=vi để nhận diện tiếng Việt
        string url = "https://api.deepgram.com/v1/listen?model=nova-2-general&language=vi&smart_format=true";
        byte[] audioData = File.ReadAllBytes(audioPath);
        Debug.Log($"📂 Audio file size: {audioData.Length} bytes");

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(audioData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "audio/wav");
        www.SetRequestHeader("Authorization", "Token " + config.deepgram_api_key);

        float startTime = Time.time;
        yield return www.SendWebRequest();
        float duration = Time.time - startTime;

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ Deepgram response trong {duration:F2}s");
            string json = www.downloadHandler.text;
            Debug.Log("📋 Raw JSON response: " + json); // Debug để xem response
            string transcript = ParseDeepgramResponse(json);
            Debug.Log("📝 Extracted transcript: '" + transcript + "'");
            callback(transcript);
        }
        else
        {
            Debug.LogError("❌ Deepgram Error: " + www.error);
            Debug.LogError("Response: " + www.downloadHandler.text);
        }
    }

    string ParseDeepgramResponse(string json)
    {
        // Deepgram trả JSON dạng { "results": { "channels": [ { "alternatives": [ { "transcript": "..." } ] } ] } }
        var root = JsonUtility.FromJson<DeepgramResponseRoot>(json);
        if (root.results.channels.Length > 0 && root.results.channels[0].alternatives.Length > 0)
        {
            return root.results.channels[0].alternatives[0].transcript;
        }
        return "";
    }

    IEnumerator SendToGemini(string userText, Action<string> callback)
    {
        Debug.Log("🧠 Bắt đầu gửi đến Gemini AI...");
        // Sử dụng model đúng: gemini-1.5-flash hoặc gemini-1.5-pro
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={config.gemini_api_key}";

        string prompt = config.gemini_prompt + "\nNgười chơi hỏi: " + userText;
        string jsonBody = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"}]}]}";

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        float startTime = Time.time;
        yield return www.SendWebRequest();
        float duration = Time.time - startTime;

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ Gemini response trong {duration:F2}s");
            callback(ParseGeminiResponse(www.downloadHandler.text));
        }
        else
        {
            Debug.LogError("❌ Gemini Error: " + www.error);
            Debug.LogError("Response: " + www.downloadHandler.text);
        }
    }

    string ParseGeminiResponse(string json)
    {
        Debug.Log("🔍 Parsing Gemini JSON: " + json); // Debug để xem raw response
        
        try
        {
            // Cách 1: Parse JSON structure đúng cách cho format mới
            if (json.Contains("\"text\":"))
            {
                // Tìm "text" field và extract content
                int textStart = json.IndexOf("\"text\":");
                if (textStart != -1)
                {
                    // Tìm dấu " đầu tiên sau "text":
                    int quoteStart = json.IndexOf("\"", textStart + 7) + 1;
                    
                    // Tìm dấu " kết thúc (có thể có escape characters)
                    int quoteEnd = quoteStart;
                    while (quoteEnd < json.Length)
                    {
                        if (json[quoteEnd] == '"' && (quoteEnd == 0 || json[quoteEnd - 1] != '\\'))
                        {
                            break;
                        }
                        quoteEnd++;
                    }
                    
                    if (quoteStart > 0 && quoteEnd > quoteStart)
                    {
                        string result = json.Substring(quoteStart, quoteEnd - quoteStart);
                        // Xử lý escape characters
                        result = result.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
                        // Loại bỏ \n cuối nếu có
                        result = result.TrimEnd('\n');
                        
                        Debug.Log("✅ Extracted text: '" + result + "'");
                        return result;
                    }
                }
            }
            
            // Fallback - không tìm thấy text
            Debug.LogWarning("⚠️ Không tìm thấy text field hợp lệ");
            return "";
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Lỗi parse JSON: " + e.Message);
            Debug.LogError("JSON content: " + json);
            return "";
        }
    }

    IEnumerator SendToElevenLabs(string text)
    {
        Debug.Log("🎵 Bắt đầu tạo voice với ElevenLabs...");
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{config.elevenlabs_voice_id}?optimize_streaming_latency=4&model_id=eleven_flash_v2_5";
        string jsonBody = "{\"text\":\"" + text + "\"}";

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("xi-api-key", config.elevenlabs_api_key);

        float startTime = Time.time;
        yield return www.SendWebRequest();
        float duration = Time.time - startTime;

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ ElevenLabs response trong {duration:F2}s");
            byte[] audioData = www.downloadHandler.data;
            Debug.Log($"🎧 Audio data size: {audioData.Length} bytes");
            
            string tempFile = Path.Combine(Application.persistentDataPath, "npc_response.mp3");
            File.WriteAllBytes(tempFile, audioData);
            Debug.Log("💾 Đã lưu audio file: " + tempFile);
            
            StartCoroutine(PlayAudio(tempFile));
        }
        else
        {
            Debug.LogError("❌ ElevenLabs Error: " + www.error);
            Debug.LogError("Response: " + www.downloadHandler.text);
        }
    }

    IEnumerator PlayAudio(string path)
    {
        Debug.Log("🎵 Bắt đầu phát audio: " + path);
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Audio loaded thành công!");
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                npcAudioSource.clip = clip;
                npcAudioSource.Play();
                Debug.Log("🔊 Đang phát audio...");
            }
            else
            {
                Debug.LogError("❌ Lỗi load audio: " + www.error);
                Debug.LogError("File path: " + path);
                
                // Kiểm tra file có tồn tại không
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    Debug.Log($"File tồn tại, size: {fileInfo.Length} bytes");
                }
                else
                {
                    Debug.LogError("File không tồn tại!");
                }
            }
        }
    }

    // ---- Utility để lưu WAV ----
    void SaveWav(string filePath, AudioClip clip)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        byte[] wavData = WavUtility.FromAudioClip(clip);
        File.WriteAllBytes(filePath, wavData);
        Debug.Log("Saved WAV: " + filePath);
    }
}

// Deepgram JSON classes
[System.Serializable]
public class DeepgramResponseRoot { public DeepgramResults results; }
[System.Serializable]
public class DeepgramResults { public DeepgramChannel[] channels; }
[System.Serializable]
public class DeepgramChannel { public DeepgramAlternative[] alternatives; }
[System.Serializable]
public class DeepgramAlternative { public string transcript; }

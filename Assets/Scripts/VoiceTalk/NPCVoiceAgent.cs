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
    private int sampleRate = 16000;

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
        string sttText = null;
        yield return SendToDeepgram(audioPath, result => sttText = result);

        if (!string.IsNullOrEmpty(sttText))
        {
            Debug.Log("STT: " + sttText);

            string aiResponse = null;
            yield return SendToGemini(sttText, result => aiResponse = result);

            Debug.Log("Gemini: " + aiResponse);

            yield return SendToElevenLabs(aiResponse);
        }
    }

    IEnumerator SendToDeepgram(string audioPath, Action<string> callback)
    {
        string url = "https://api.deepgram.com/v1/listen?model=whisper-large";
        byte[] audioData = File.ReadAllBytes(audioPath);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(audioData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "audio/wav");
        www.SetRequestHeader("Authorization", "Token " + config.deepgram_api_key);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            string transcript = ParseDeepgramResponse(json);
            callback(transcript);
        }
        else
        {
            Debug.LogError("Deepgram Error: " + www.error);
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
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-2.5:generateContent?key={config.gemini_api_key}";

        string prompt = config.gemini_prompt + "\nNgười chơi hỏi: " + userText;
        string jsonBody = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"}]}]}";

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            callback(ParseGeminiResponse(www.downloadHandler.text));
        }
        else
        {
            Debug.LogError("Gemini Error: " + www.error);
        }
    }

    string ParseGeminiResponse(string json)
    {
        // Parse cơ bản (Gemini trả JSON phức tạp)
        if (json.Contains("\"text\":"))
        {
            int start = json.IndexOf("\"text\":") + 8;
            int end = json.IndexOf("\"", start);
            return json.Substring(start, end - start);
        }
        return json;
    }

    IEnumerator SendToElevenLabs(string text)
    {
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{config.elevenlabs_voice_id}?optimize_streaming_latency=4&model_id=eleven_flash_v2_5";
        string jsonBody = "{\"text\":\"" + text + "\"}";

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("xi-api-key", config.elevenlabs_api_key);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            byte[] audioData = www.downloadHandler.data;
            string tempFile = Path.Combine(Application.persistentDataPath, "npc_response.mp3");
            File.WriteAllBytes(tempFile, audioData);
            StartCoroutine(PlayAudio(tempFile));
        }
        else
        {
            Debug.LogError("ElevenLabs Error: " + www.error);
        }
    }

    IEnumerator PlayAudio(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                npcAudioSource.clip = clip;
                npcAudioSource.Play();
            }
            else
            {
                Debug.LogError(www.error);
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

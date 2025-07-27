using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ElevenLabsTTS : MonoBehaviour
{
    [System.Serializable]
    public enum VoiceID
    {
        MCViet = 0, 
        Vietdung = 1,  
        Adam = 2,       // pNInz6obpgDQGcFmaJgB
        Bella = 3,      // EXAVITQu4vr4xnSDxMaL
        Antoni = 4,     // ErXwobaYiN019PkySvjV
        Elli = 5,       // MF3mGyEYCl7XYWbV9V6O
        Josh = 6,       // TxGEqnHWrfWFTfGW9XjX
        Arnold = 7,     // VR6AewLTigWG4xSOukaG
        Sam = 8,        // yoZ06aMxZJJ28mfd3POQ
        BUPPIX = 9      // BUPPIXeDaJWBz696iXRS - Voice bạn yêu cầu
    }

    public static string GetVoiceIDString(VoiceID voiceId)
    {
        switch (voiceId)
        {
            case VoiceID.MCViet: return "NSzi72jFi7P1JqCwPRuM";
            case VoiceID.Vietdung: return "ywBZEqUhld86Jeajq94o";
            case VoiceID.Adam: return "pNInz6obpgDQGcFmaJgB";
            case VoiceID.Bella: return "EXAVITQu4vr4xnSDxMaL";
            case VoiceID.Antoni: return "ErXwobaYiN019PkySvjV";
            case VoiceID.Elli: return "MF3mGyEYCl7XYWbV9V6O";
            case VoiceID.Josh: return "TxGEqnHWrfWFTfGW9XjX";
            case VoiceID.Arnold: return "VR6AewLTigWG4xSOukaG";
            case VoiceID.Sam: return "yoZ06aMxZJJ28mfd3POQ";
            case VoiceID.BUPPIX: return "BUPPIXeDaJWBz696iXRS";
            default: return "21m00Tcm4TlvDq8ikWAM"; // Default to Rachel
        }
    }

    [System.Serializable]
    public class TTSRequest
    {
        public string text;
        public string model_id = "eleven_flash_v2_5"; // Faster model
        public VoiceSettings voice_settings;
        public int optimize_streaming_latency = 4; // Optimize for low latency
        public string output_format = "mp3_22050_32"; // Lower quality for faster processing
    }

    [System.Serializable]
    public class VoiceSettings
    {
        public float stability = 0.5f;
        public float similarity_boost = 0.5f;
    }

    public static IEnumerator GetTextToSpeechAudio(string text, string apiKey, VoiceID voiceId, System.Action<AudioClip> callback)
    {
        string voiceIdString = GetVoiceIDString(voiceId);
        yield return GetTextToSpeechAudio(text, apiKey, voiceIdString, callback);
    }

    public static IEnumerator GetTextToSpeechAudio(string text, string apiKey, string voiceId, System.Action<AudioClip> callback)
    {
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}";
        
        TTSRequest requestData = new TTSRequest
        {
            text = text,
            voice_settings = new VoiceSettings()
        };
        
        string jsonData = JsonUtility.ToJson(requestData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("xi-api-key", apiKey);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError("ElevenLabs TTS Error: " + www.error);
                callback?.Invoke(null);
            } 
            else 
            {
                // Save audio data to temp file and load as AudioClip
                byte[] audioData = www.downloadHandler.data;
                string tempPath = Application.persistentDataPath + "/temp_tts.mp3";
                System.IO.File.WriteAllBytes(tempPath, audioData);
                
                // Load audio using UnityWebRequest (supports MP3)
                using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + tempPath, AudioType.MPEG))
                {
                    yield return audioRequest.SendWebRequest();
                    
                    if (audioRequest.result == UnityWebRequest.Result.Success)
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                        callback?.Invoke(clip);
                    }
                    else
                    {
                        Debug.LogError("Audio loading error: " + audioRequest.error);
                        callback?.Invoke(null);
                    }
                }
                
                // Clean up temp file
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
        }
    }
}

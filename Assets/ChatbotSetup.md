# Gemini AI Chatbot với ElevenLabs TTS

Hướng dẫn thiết lập chatbot AI sử dụng Gemini API và ElevenLabs TTS trong Unity 3D.

## Thiết lập API Keys

1. **Tạo file API Keys:**
   - Mở file `Assets/Scripts/api_keys.json`
   - Thay thế `YOUR_GEMINI_API_KEY_HERE` bằng API key của bạn từ Google AI Studio
   - Thay thế `YOUR_ELEVENLABS_API_KEY_HERE` bằng API key của bạn từ ElevenLabs

2. **Lấy Gemini API Key:**
   - Truy cập: https://aistudio.google.com/app/apikey
   - Tạo API key mới
   - Copy và paste vào file api_keys.json

3. **Lấy ElevenLabs API Key:**
   - Truy cập: https://elevenlabs.io/
   - Tạo tài khoản và lấy API key
   - Copy và paste vào file api_keys.json

## Thiết lập UI trong Unity

### 1. Tạo Canvas Chat
1. Tạo Canvas mới (UI > Canvas)
2. Thêm các thành phần sau vào Canvas:

### 2. Input Field (Nhập câu hỏi)
```
- Tạo UI > Input Field - TextMeshPro
- Đặt tên: "ChatInputField"
- Placeholder text: "Nhập câu hỏi của bạn..."
```

### 3. Send Button (Nút gửi)
```
- Tạo UI > Button - TextMeshPro
- Đặt tên: "SendButton"
- Text: "Gửi"
```

### 4. Toggle TTS (Bật/tắt đọc)
```
- Tạo UI > Toggle
- Đặt tên: "TTSToggle"
- Label text: "Đọc câu trả lời"
```

### 5. Voice Selector (Tùy chọn - Chọn giọng nói)
```
- Tạo UI > Dropdown - TextMeshPro
- Đặt tên: "VoiceDropdown"
```

### 6. Chat Display (Hiển thị chat)
```
- Tạo UI > Scroll View
- Bên trong Content, tạo UI > Text - TextMeshPro
- Đặt tên: "ChatDisplayText"
- Thiết lập:
  - Text Wrapping: Enabled
  - Overflow: Overflow
  - Vertical Alignment: Top
  - Horizontal Alignment: Left
```

### 6. Thiết lập ScrollRect
```
- Trên Scroll View component:
  - Content: Gán Content object
  - Vertical: Checked
  - Horizontal: Unchecked
  - Movement Type: Elastic
  - Scroll Sensitivity: 1
```

### 7. Thiết lập Voice Selector (Tùy chọn)
```
- Tạo Empty GameObject, đặt tên "VoiceSelector"
- Gán script VoiceSelector.cs
- Thiết lập:
  - Voice Dropdown: Gán VoiceDropdown
  - Chatbot: Gán ChatbotManager GameObject
```

## Thiết lập Script

### 1. Gán GeminiChatbot Script
1. Tạo Empty GameObject mới, đặt tên "ChatbotManager"
2. Gán script `GeminiChatbot.cs` vào GameObject này
3. Thiết lập các tham số trong Inspector:

```
API Keys Json: Gán file api_keys.json
Input Field: Gán ChatInputField
Chat Display Text: Gán ChatDisplayText
Send Button: Gán SendButton
TTS Toggle: Gán TTSToggle
Chat Scroll Rect: Gán ScrollView component
Bot Instructions: "Bạn là một trợ lý AI thông minh và hữu ích..."
Max Chat History: 10
Selected Voice: BUPPIX (hoặc voice khác bạn muốn)
```

### 2. Cấu hình Audio
- Script sẽ tự động tạo AudioSource component
- Đảm bảo AudioListener có trong scene (thường ở Main Camera)

## Tính năng

### 1. Chat cơ bản
- Nhập câu hỏi vào Input Field
- Nhấn nút "Gửi" hoặc Enter để gửi
- Câu trả lời sẽ hiển thị trong Chat Display

### 2. Lịch sử chat
- Chat sẽ hiển thị cả câu hỏi và câu trả lời
- Có timestamp cho mỗi tin nhắn
- Tự động cuộn xuống tin nhắn mới nhất
- Giới hạn số lượng tin nhắn hiển thị (có thể cấu hình)

### 3. Text-to-Speech (TTS)
- Bật/tắt bằng Toggle
- Khi bật, AI sẽ đọc mỗi câu trả lời
- Sử dụng ElevenLabs API cho chất lượng giọng nói cao

### 4. Chọn giọng nói
- Sử dụng dropdown để chọn giọng nói yêu thích
- Có 10 giọng nói khác nhau để lựa chọn:
  - Rachel, Drew, Adam, Bella, Antoni, Elli, Josh, Arnold, Sam, BUPPIX

## Voice IDs có sẵn

```csharp
Rachel: "21m00Tcm4TlvDq8ikWAM" (Nữ, Mỹ)
Drew: "29vD33N1CtxCmqQRPOHJ" (Nam, Mỹ)
Adam: "pNInz6obpgDQGcFmaJgB" (Nam, Mỹ)
Bella: "EXAVITQu4vr4xnSDxMaL" (Nữ, Mỹ)
Antoni: "ErXwobaYiN019PkySvjV" (Nam, Ba Lan)
Elli: "MF3mGyEYCl7XYWbV9V6O" (Nữ, Mỹ)
Josh: "TxGEqnHWrfWFTfGW9XjX" (Nam, Mỹ)
Arnold: "VR6AewLTigWG4xSOukaG" (Nam, Mỹ)
Sam: "yoZ06aMxZJJ28mfd3POQ" (Nam, Mỹ)
BUPPIX: "BUPPIXeDaJWBz696iXRS" (Giọng đặc biệt - voice bạn yêu cầu)
```

## Voice IDs phổ biến của ElevenLabs

```csharp
// Thay đổi selectedVoice trong GeminiChatbot Inspector
// Hoặc sử dụng VoiceSelector dropdown để thay đổi trong game
```

## Xử lý lỗi phổ biến

1. **"API key not set"**: Kiểm tra file api_keys.json
2. **"TTS Error"**: Kiểm tra ElevenLabs API key và voice ID
3. **"No response"**: Kiểm tra kết nối internet và Gemini API key
4. **Audio không phát**: Kiểm tra AudioSource và AudioListener

## Mở rộng

Bạn có thể mở rộng chatbot bằng cách:
- Thêm avatar AI với animation khi nói
- Lưu trữ lịch sử chat vào file
- Thêm nhiều giọng nói khác nhau
- Tích hợp với game mechanics khác

## Lưu ý bảo mật

- Không commit file api_keys.json lên Git
- Thêm api_keys.json vào .gitignore
- Sử dụng Environment Variables cho production build

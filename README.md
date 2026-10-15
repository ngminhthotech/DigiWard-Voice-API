# DigiWard Voice AI API


## 1. Tổng quan

DigiWard Voice AI API là hệ thống trợ lý AI bằng giọng nói dành cho nền tảng chính quyền số.


Chức năng:

- Người dân đặt câu hỏi bằng giọng nói.
- AI nhận diện nội dung.
- AI trả lời thủ tục hành chính.
- Chuyển câu trả lời thành giọng nói.



## 2. Kiến trúc hệ thống


Voice Input

↓

Gemini Audio Speech Recognition

↓

Gemini AI Response

↓

ElevenLabs Text To Speech

↓

Voice Output



## 3. Công nghệ

Backend:

- ASP.NET Core Web API
- C#


AI:

- Google Gemini API
- ElevenLabs Voice API



## 4. API


### Health Check

GET

/api/v1/voice/test


---

### Text AI

POST

/api/v1/voice/ask


Body:

{
 "text":"Thủ tục đăng ký khai sinh cần gì?"
}



---

### Speech To Text

POST

/api/v1/voice/transcribe


Form Data:

audio = file mp3



---

### Full Voice Chat

POST

/api/v1/voice/chat


Form Data:


audio:
file mp3


sessionId:
user001



Response:


{
 "success":true,
 "question":"",
 "answer":"",
 "audioBase64":"",
 "mimeType":"audio/mpeg"
}



## 5. Chạy hệ thống


Yêu cầu:

.NET 8 SDK


Restore:

dotnet restore


Run:

dotnet run



## 6. Cấu hình API Key


Cần cấu hình:


Google Gemini API Key


ElevenLabs API Key



Không commit API Key lên repository.
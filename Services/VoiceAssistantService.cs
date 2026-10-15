using DigiWard.Voice.API.Models;


namespace DigiWard.Voice.API.Services;


public class VoiceAssistantService
{

    private readonly GeminiService _geminiService;

    private readonly ElevenLabsService _elevenLabsService;

    private readonly ConversationMemoryService _memoryService;




    public VoiceAssistantService(
        GeminiService geminiService,
        ElevenLabsService elevenLabsService,
        ConversationMemoryService memoryService
    )
    {
        _geminiService = geminiService;

        _elevenLabsService = elevenLabsService;

        _memoryService = memoryService;
    }








    // =====================================================
    // FULL VOICE AI FLOW
    //
    // Text
    // ↓
    // Memory
    // ↓
    // Gemini
    // ↓
    // ElevenLabs
    // ↓
    // VoiceResponse
    //
    // =====================================================


    public async Task<VoiceResponse> ProcessText(
        string sessionId,
        string text
    )
    {


        // ===============================
        // 1. Save user message
        // ===============================


        _memoryService.AddMessage(
            sessionId,
            "user",
            text
        );








        // ===============================
        // 2. Conversation history
        // ===============================


        var history =
            _memoryService.BuildContext(
                sessionId
            );








        // ===============================
        // 3. Gemini prompt
        // ===============================


        var prompt =
        $"""
        Bạn là trợ lý AI DigiWard của chính quyền số phường/xã.

        Nhiệm vụ:
        - Hỗ trợ người dân về thủ tục hành chính.
        - Hướng dẫn hồ sơ, quy trình.
        - Trả lời bằng tiếng Việt.

        Quy tắc:
        - Trả lời ngắn gọn.
        - Không quá 150 từ.
        - Không tự tạo quy định pháp luật.
        - Nếu thiếu thông tin hãy hỏi lại.


        Lịch sử hội thoại:
        {history}


        Người dân hỏi:
        {text}
        """;








        // ===============================
        // 4. Gemini Answer
        // ===============================


        var answer =
            await _geminiService.GenerateText(
                prompt
            );








        // ===============================
        // 5. Save AI message
        // ===============================


        _memoryService.AddMessage(
            sessionId,
            "assistant",
            answer
        );








        // ===============================
        // 6. ElevenLabs Voice
        // ===============================


        var audio =
            await _elevenLabsService.TextToSpeech(
                answer
            );








        // ===============================
        // 7. Return full result
        // ===============================


        return new VoiceResponse
        {

            Question = text,

            Answer = answer,

            Audio = audio

        };

    }


}
using DigiWard.Voice.API.Models;
using DigiWard.Voice.API.Services;
using Microsoft.AspNetCore.Mvc;


namespace DigiWard.Voice.API.Controllers;


[ApiController]
[Route("api/v1/voice")]
public class VoiceController : ControllerBase
{

    private readonly ElevenLabsService _elevenLabsService;

    private readonly GeminiService _geminiService;

    private readonly GeminiAudioService _geminiAudioService;

    private readonly VoiceAssistantService _voiceAssistantService;




    public VoiceController(
        ElevenLabsService elevenLabsService,
        GeminiService geminiService,
        GeminiAudioService geminiAudioService,
        VoiceAssistantService voiceAssistantService
    )
    {
        _elevenLabsService = elevenLabsService;

        _geminiService = geminiService;

        _geminiAudioService = geminiAudioService;

        _voiceAssistantService = voiceAssistantService;
    }








    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            success = true,
            message = "DigiWard Voice Bot Ready",
            service = "Voice API",
            time = DateTime.Now
        });
    }









    [HttpPost("speak")]
    public async Task<IActionResult> Speak(
        [FromBody] TextToSpeechRequest request
    )
    {

        if(request == null ||
           string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new
            {
                success = false,
                message = "Text không được rỗng"
            });
        }



        var audio =
            await _elevenLabsService.TextToSpeech(
                request.Text
            );



        return File(
            audio,
            "audio/mpeg",
            "voice.mp3"
        );

    }









    [HttpPost("ask")]
    public async Task<IActionResult> Ask(
        [FromBody] TextToSpeechRequest request
    )
    {

        if(request == null ||
           string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new
            {
                success = false,
                message = "Text không được rỗng"
            });
        }



        var answer =
            await _geminiService.GenerateText(
                request.Text
            );



        return Ok(new
        {
            success = true,
            question = request.Text,
            answer = answer
        });

    }









    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe(
        IFormFile audio
    )
    {

        if(audio == null ||
           audio.Length == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Chưa có file âm thanh"
            });
        }



        var text =
            await _geminiAudioService.Transcribe(
                audio
            );



        return Ok(new
        {
            success = true,
            text = text
        });

    }









    // =====================================================
    // FULL VOICE CHAT
    //
    // Audio
    // ↓
    // Gemini Audio
    // ↓
    // Gemini AI
    // ↓
    // ElevenLabs
    // ↓
    // JSON
    //
    // =====================================================


    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        IFormFile audio,
        string sessionId = "guest"
    )
    {

        if(audio == null ||
           audio.Length == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Chưa có file âm thanh"
            });
        }





        try
        {

            // 1. Audio -> Text

            var question =
                await _geminiAudioService.Transcribe(
                    audio
                );







            // 2. AI -> Voice

            var result =
                await _voiceAssistantService.ProcessText(
                    sessionId,
                    question
                );







            // 3. Convert audio

            var audioBase64 =
                Convert.ToBase64String(
                    result.Audio
                );







            return Ok(new
            {

                success = true,


                question = result.Question,


                answer = result.Answer,


                audioBase64 = audioBase64,


                mimeType = "audio/mpeg"

            });

        }
        catch(Exception ex)
        {

            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });

        }

    }


}
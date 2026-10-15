using DigiWard.Voice.API.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;


namespace DigiWard.Voice.API.Services;


public class ElevenLabsService
{

    private readonly HttpClient _httpClient;

    private readonly ElevenLabsSettings _settings;





    public ElevenLabsService(
        HttpClient httpClient,
        IOptions<ElevenLabsSettings> settings
    )
    {
        _httpClient = httpClient;

        _settings = settings.Value;


        Console.WriteLine(
            $"ElevenLabs VoiceId: {_settings.VoiceId}"
        );
    }








    public async Task<byte[]> TextToSpeech(
        string text
    )
    {


        text = CleanText(text);




        var url =
            $"https://api.elevenlabs.io/v1/text-to-speech/{_settings.VoiceId}?output_format=mp3_44100_128";






        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                url
            );





        request.Headers.Add(
            "xi-api-key",
            _settings.ApiKey
        );






        var body = new
        {

            text = text,


            // Model tối ưu tiếng Việt
            model_id = "eleven_flash_v2_5",




            voice_settings = new
            {

                // ổn định giọng đọc
                stability = 0.75,


                // giữ chất giọng
                similarity_boost = 0.85,


                // hạn chế diễn cảm quá mức
                style = 0.05,


                use_speaker_boost = true

            }

        };






        request.Content =
            JsonContent.Create(body);







        var response =
            await _httpClient.SendAsync(request);







        if(!response.IsSuccessStatusCode)
        {

            var error =
                await response.Content.ReadAsStringAsync();


            throw new Exception(
                $"ElevenLabs Error: {response.StatusCode} - {error}"
            );

        }







        return await response.Content
            .ReadAsByteArrayAsync();

    }








    private string CleanText(
        string text
    )
    {

        if(string.IsNullOrWhiteSpace(text))
            return "";




        return text
            .Replace("*", "")
            .Replace("#", "")
            .Replace("<break>", "")
            .Replace("\n\n\n", "\n\n")
            .Trim();

    }

}
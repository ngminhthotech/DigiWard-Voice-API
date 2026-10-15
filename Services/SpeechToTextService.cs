using System.Net.Http.Headers;

namespace DigiWard.Voice.API.Services;


public class SpeechToTextService
{

    private readonly HttpClient _httpClient;

    private readonly IConfiguration _configuration;


    public SpeechToTextService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }



    public async Task<string> Transcribe(
        IFormFile audio)
    {

        using var content =
            new MultipartFormDataContent();



        var fileContent =
            new StreamContent(
                audio.OpenReadStream()
            );


        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(
                audio.ContentType ?? "audio/mpeg"
            );



        content.Add(
            fileContent,
            "file",
            audio.FileName
        );



        content.Add(
            new StringContent(
                "whisper-1"
            ),
            "model"
        );


        content.Add(
            new StringContent(
                "vi"
            ),
            "language"
        );



        var apiKey =
            _configuration["OpenAI:ApiKey"];



        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey
            );



        var response =
            await _httpClient.PostAsync(
                "https://api.openai.com/v1/audio/transcriptions",
                content
            );



        if(!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync();


            throw new Exception(error);
        }



        var result =
            await response.Content
            .ReadFromJsonAsync<WhisperResponse>();


        return result!.Text;

    }

}



public class WhisperResponse
{
    public string Text {get;set;} = "";
}
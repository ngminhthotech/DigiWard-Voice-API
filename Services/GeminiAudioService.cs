using System.Text;
using System.Text.Json;


namespace DigiWard.Voice.API.Services;


public class GeminiAudioService
{

    private readonly HttpClient _httpClient;

    private readonly IConfiguration _configuration;



    public GeminiAudioService(
        HttpClient httpClient,
        IConfiguration configuration
    )
    {
        _httpClient = httpClient;

        _configuration = configuration;
    }







    // =====================================================
    // AUDIO -> TEXT
    //
    // Gemini Multimodal
    //
    // Input:
    // mp3 / wav
    //
    // Output:
    // text tiếng Việt
    //
    // =====================================================


    public async Task<string> Transcribe(
        IFormFile audio
    )
    {


        var apiKey =
            _configuration["GeminiAI:ApiKey"];



        var model =
            _configuration["GeminiAI:Model"]
            ??
            "gemini-2.5-flash";





        // ===============================
        // đọc file audio
        // ===============================


        var bytes =
            new byte[audio.Length];



        using(var stream = audio.OpenReadStream())
        {
            await stream.ReadAsync(
                bytes,
                0,
                bytes.Length
            );
        }





        var base64 =
            Convert.ToBase64String(bytes);








        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";










        // ===============================
        // Gemini multimodal request
        // ===============================


        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {

                        // =====================
                        // AUDIO DATA
                        // =====================

                        new
                        {
                            inline_data = new
                            {

                                mime_type =
                                    GetMimeType(
                                        audio.FileName
                                    ),


                                data = base64

                            }
                        },





                        // =====================
                        // PROMPT
                        // =====================

                        new
                        {
                            text =
                            """
                            Hãy nghe đoạn âm thanh này.

                            Chuyển nội dung người nói thành văn bản tiếng Việt.

                            Chỉ trả về câu nói của người dùng.
                            Không giải thích thêm.
                            """
                        }

                    }
                }
            }
        };









        var json =
            JsonSerializer.Serialize(body);







        var response =
            await _httpClient.PostAsync(
                url,
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            );








        if(!response.IsSuccessStatusCode)
        {

            var error =
                await response.Content.ReadAsStringAsync();


            throw new Exception(
                $"Gemini Audio Error: {error}"
            );

        }










        var result =
            await response.Content.ReadAsStringAsync();







        using var doc =
            JsonDocument.Parse(result);







        var text =
            doc
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();







        return text ?? "";

    }








    // =====================================================
    // Detect audio MIME
    // =====================================================


    private string GetMimeType(
        string fileName
    )
    {

        var extension =
            Path.GetExtension(fileName)
            .ToLower();



        return extension switch
        {

            ".mp3" =>
                "audio/mpeg",


            ".wav" =>
                "audio/wav",


            ".m4a" =>
                "audio/mp4",


            ".webm" =>
                "audio/webm",


            ".ogg" =>
                "audio/ogg",


            _ =>
                "audio/mpeg"

        };

    }


}
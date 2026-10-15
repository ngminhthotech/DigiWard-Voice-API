using System.Text;
using System.Text.Json;


namespace DigiWard.Voice.API.Services;


public class GeminiService
{

    private readonly HttpClient _httpClient;

    private readonly IConfiguration _configuration;

    private readonly KnowledgeService _knowledgeService;




    public GeminiService(
        HttpClient httpClient,
        IConfiguration configuration,
        KnowledgeService knowledgeService
    )
    {
        _httpClient = httpClient;

        _configuration = configuration;

        _knowledgeService = knowledgeService;
    }







    public async Task<string> GenerateText(
        string prompt
    )
    {

        var apiKey =
            _configuration["GeminiAI:ApiKey"];



        var model =
            _configuration["GeminiAI:Model"]
            ??
            "gemini-2.5-flash";





        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";







        var systemPrompt =
        """
        Bạn là DigiWard AI Assistant.

        Vai trò:
        - Trợ lý chính quyền số phường/xã.
        - Hỗ trợ người dân về thủ tục hành chính.

        Quy tắc:
        - Trả lời bằng tiếng Việt.
        - Xưng hô Anh/chị.
        - Ngắn gọn, dễ hiểu.
        - Hướng dẫn từng bước.
        - Không tự tạo quy định pháp luật.
        - Nếu thiếu thông tin hãy hỏi lại.

        Phong cách:
        Giống cán bộ Bộ phận Một cửa điện tử.
        """;







        // Lấy dữ liệu nghiệp vụ

        var knowledge =
            _knowledgeService.LoadKnowledge();







        var finalPrompt =
            systemPrompt
            +
            "\n\n"
            +
            "DỮ LIỆU NGHIỆP VỤ:\n"
            +
            knowledge
            +
            "\n\n"
            +
            "CÂU HỎI:\n"
            +
            prompt;







        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = finalPrompt
                        }
                    }
                }
            },


            generationConfig = new
            {
                temperature = 0.2,

                maxOutputTokens = 300
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
                $"Gemini API Error: {error}"
            );

        }







        var result =
            await response.Content.ReadAsStringAsync();







        using var doc =
            JsonDocument.Parse(result);







        return
            doc
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()
            ??
            "";

    }


}
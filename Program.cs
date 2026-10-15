using DigiWard.Voice.API.Configurations;
using DigiWard.Voice.API.Services;


var builder = WebApplication.CreateBuilder(args);



// ===============================
// Controllers
// ===============================

builder.Services.AddControllers();




// ===============================
// Configuration
// ===============================

builder.Services.Configure<ElevenLabsSettings>(
    builder.Configuration.GetSection("ElevenLabs")
);





// ===============================
// Core Services
// ===============================


builder.Services.AddSingleton<KnowledgeService>();

builder.Services.AddSingleton<ConversationMemoryService>();





// ===============================
// AI Services
// ===============================


builder.Services.AddHttpClient<GeminiService>();

builder.Services.AddHttpClient<GeminiAudioService>();





// ===============================
// Voice Services
// ===============================


builder.Services.AddHttpClient<ElevenLabsService>();

builder.Services.AddScoped<VoiceAssistantService>();





// ===============================
// Legacy STT
// giữ lại để test
// ===============================

builder.Services.AddHttpClient<SpeechToTextService>();





var app = builder.Build();





app.UseHttpsRedirection();


app.MapControllers();


app.Run();
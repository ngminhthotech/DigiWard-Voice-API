namespace DigiWard.Voice.API.Models;


public class VoiceResponse
{

    public string Question { get; set; } = "";


    public string Answer { get; set; } = "";


    public byte[] Audio { get; set; } = Array.Empty<byte>();

}
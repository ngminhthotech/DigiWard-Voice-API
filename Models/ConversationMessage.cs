namespace DigiWard.Voice.API.Models;


public class ConversationMessage
{

    public string SessionId {get;set;} = "";


    public string Role {get;set;} = "";
    

    public string Content {get;set;} = "";


    public DateTime CreatedAt {get;set;}
        = DateTime.Now;

}
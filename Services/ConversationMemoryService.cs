using DigiWard.Voice.API.Models;


namespace DigiWard.Voice.API.Services;


public class ConversationMemoryService
{

    private readonly List<ConversationMessage> _messages = new();




    public void AddMessage(
        string sessionId,
        string role,
        string content
    )
    {

        _messages.Add(
            new ConversationMessage
            {
                SessionId = sessionId,
                Role = role,
                Content = content,
                CreatedAt = DateTime.Now
            }
        );

    }





    public List<ConversationMessage> GetHistory(
        string sessionId
    )
    {

        return _messages
            .Where(x =>
                x.SessionId == sessionId
            )
            .OrderBy(x =>
                x.CreatedAt
            )
            .ToList();

    }







    public string BuildContext(
        string sessionId
    )
    {

        var history =
            GetHistory(sessionId);



        if(history.Count == 0)
            return "";



        return string.Join(
            "\n",
            history.Select(x =>
                $"{x.Role}: {x.Content}"
            )
        );

    }

}
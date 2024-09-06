namespace Chat.APIs.Models.Domain;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public string Content { get; set; }

    public string Username { get; set; }

    public Conversation Conversation { get; set; }

    public DateTime CreatedOn { get; set; }

}

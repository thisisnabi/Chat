namespace Chat.APIs.Models.Domain
{
    public class ConversationUser
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public string UserName { get; set; }

        public DateTime JoinedOn { get; set; }

        public Conversation  Conversation { get; set; }

    }
}

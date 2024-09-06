namespace Chat.APIs.Models.Domain
{
    public class Conversation
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public bool IsPrivate { get; private set; }

        public ICollection<Message> Messages { get; private set; }
        public ICollection<ConversationUser> Users { get; private set; }

        public void AddUser(string userName)
        {
            Users ??= new List<ConversationUser>();

            if (IsPrivate && Users.Count > 1)
                throw new InvalidOperationException("Conversation is private!");

            Users.Add(new ConversationUser
            {
                JoinedOn = DateTime.Now,
                UserName = userName,
            });
        }
        
        /// <summary>
        /// Create a public conversation
        /// </summary>
        /// <returns></returns>
        public static Conversation Create()
            => new Conversation
            {
                IsPrivate = false,
                Title = "Joined to the public conversation"
            };


        /// <summary>
        /// Create a private conversation
        /// </summary>
        /// <returns></returns>
        public static Conversation Create(string username_left, string username_right)
        {
            return new Conversation
            {
                IsPrivate = true,
                Title = $"Talk ({username_left},{username_right})",
                Users = new List<ConversationUser> {

                    new ConversationUser
                    {
                        UserName = username_left,
                        JoinedOn= DateTime.UtcNow,
                    },
                    new ConversationUser
                    {
                        UserName = username_right,
                        JoinedOn= DateTime.UtcNow,
                    }
                }
            };
        }

        public Message AddMessage(string message, string username)
        {
            Messages ??= [];
            var messageModel = new Message
            {
                Content = message,
                CreatedOn = DateTime.UtcNow,
                Username = username,
            };
            Messages.Add(messageModel);

            return messageModel;
        }
    }
}

using Azure.Core;
using Chat.APIs.Common;
using Chat.APIs.SharedData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Chat.APIs.Hubs
{
    public class ChatHub(UserService userService, ChatDbContext dbContext) : Hub
    {
        private readonly UserService _userService = userService;
        private readonly ChatDbContext _chatContext = dbContext;

        public async Task SendMessage(Guid ConversationId, string Message)
        {
            var username = GetCallerUserName();
            if (username is null)
                return;
             
            var conversation = await dbContext.Conversations
                                  .Include(d => d.Users)
                                  .FirstOrDefaultAsync(x => x.Id == ConversationId);

            if (conversation is null)
                return;


            if (!conversation.Users.Any(d => d.UserName == username))
                return;

            var newMessage = conversation.AddMessage(Message, username);
            await dbContext.SaveChangesAsync();


            var usernames = conversation.Users.Select(d => d.UserName);
             
            var connectionIds =  _userService.GetUsersConnectionIds(usernames);
            if (connectionIds.Count > 0)
            {
                await Clients.Clients(connectionIds).SendAsync("NewMessage", new {
                    ConversationId = conversation.Id,
                    senderName = newMessage.Username,
                    content = newMessage.Content,
                    sentAt = newMessage.CreatedOn
                });
            }
        }
         
        public override Task OnConnectedAsync()
        {
            var username = GetCallerUserName();
            if (username is null) 
                return Task.CompletedTask;
 
            _userService.AddOnlineUser(username, Context.ConnectionId);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _userService.RemoveOnlineUserByConnectionId(Context.ConnectionId);
            return Task.CompletedTask;
        }



        public string? GetCallerUserName()

        {
            if (Context.GetHttpContext().Request.Cookies.TryGetValue("currentUserName", out string? username))
                return username;

            return null;
        }
    }
}

using Chat.APIs.Common;
using Chat.APIs.Models.Domain;
using Chat.APIs.SharedData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.APIs.REST;

public static class ConversationEndpoints
{
    public static void MapConversationEndpoint(this IEndpointRouteBuilder builder)
    {
        var groupEndpoint = builder.MapGroup("/conversation")
                                   .WithTags("Conversation Endpoints");

        groupEndpoint.MapPost("/", async (
            CreatePrivateConversationRequest request, UserService userService, ChatDbContext dbContext) =>
        {

            if (request == null ||
                !userService.ExistsUserByName(request.Me) ||
                !userService.ExistsUserByName(request.Other))
                return Results.BadRequest();

            var privateConversation = Conversation.Create(request.Me, request.Other);

            dbContext.Conversations.Add(privateConversation);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

        groupEndpoint.MapPost("/public", async (
         CreatePublicConversationRequest request, UserService userService, ChatDbContext dbContext) =>
        {

            if (request == null ||
                !userService.ExistsUserByName(request.Me) ||
                request.Others.Any(d => !userService.ExistsUserByName(d.Name)))
                return Results.BadRequest();

            var publicConversation = Conversation.Create();

            publicConversation.AddUser(request.Me);
            foreach (var other in request.Others)
            {
                publicConversation.AddUser(other.Name);
            }

            dbContext.Conversations.Add(publicConversation);
            await dbContext.SaveChangesAsync();

            return Results.Ok();
        });

        groupEndpoint.MapGet("/{username}", (string Username, UserService userService, ChatDbContext dbContext) =>
        {
            return Results.Ok(
                dbContext.Conversations
                         .Include(d => d.Users)
                         .Where(z => z.Users.Any(f => f.UserName == Username))
                         .Select(x => new
                         {
                             Id = x.Id,
                             CountIdUnreadMessages = 2,
                             Title = x.Title
                         }));
        });

        groupEndpoint.MapGet("/{conversationid}/messages", (
        [FromRoute] Guid conversationid, ChatDbContext dbContext) =>
        {
            return Results.Ok(
                dbContext.Conversations
                         .Include(d => d.Users)
                         .Where(z => z.Id == conversationid)
                         .SelectMany(x => x.Messages)
                         .OrderBy(d => d.CreatedOn)
                         .Select(g => new
                         {
                             senderName = g.Username,
                             content = g.Content,
                             sentAt = g.CreatedOn
                         }));
        });

        groupEndpoint.MapPost("/{conversationid}/messages", async (
                [FromRoute] Guid conversationid, CreateMessage request,ChatDbContext dbContext) =>
        {

            var conversation = await dbContext.Conversations
                                              .Include(d => d.Users)              
                                              .FirstOrDefaultAsync(x => x.Id ==  conversationid);
            if (conversation is null)
                return Results.BadRequest("conversation id is not valid!");


            if (!conversation.Users.Any(d => d.UserName == request.Username))
                return Results.BadRequest("user is not in conversation!");

            conversation.AddMessage(request.Content,request.Username);
            await dbContext.SaveChangesAsync();

            return Results.Ok();
        });

        groupEndpoint.MapGet("/", (UserService userService, ChatDbContext dbContext) =>
        {
            return Results.Ok(dbContext.Conversations.Include(d => d.Users).Select(x => new
            {
                Id = x.Id,
                CountIdUnreadMessages = 2,
                Title = x.Title
            }));
        });
    }


}


public record CreatePrivateConversationRequest(string Me, string Other);

public record CreatePublicConversationRequest(string Me, List<CreatePublicConversationRequestItem> Others);
public record CreatePublicConversationRequestItem(string Name);
public record CreateMessage(string Username, string Content);
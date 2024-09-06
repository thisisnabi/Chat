using Chat.APIs.Common;

namespace Chat.APIs.REST;

public static class UserEndpoints
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder builder)
    {
        var groupEndpoint = builder.MapGroup("/user")
                                   .WithTags("Conversation Endpoints");

        groupEndpoint.MapGet("/{username}", (
            string username, UserService userService) =>
        {
            if (!userService.ExistsUserByName(username))
                return Results.BadRequest();

            return Results.Ok();
        });
    }


}

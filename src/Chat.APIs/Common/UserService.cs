

namespace Chat.APIs.Common;

public class UserService
{
    private List<string> _users = [
        "thisisnabi",
        "ali_ghodrati", 
        "amihossien",
        "fatameh_ghasemi",
        "nader_javid",
        "zahra_shirzad"
    ];

    public List<OnlineUser> OnlineUsers = [];

    public bool ExistsUserByName(string userName) => _users.Contains(userName);

    public void AddOnlineUser(string username, string connectionId) => OnlineUsers.Add(new OnlineUser(username, connectionId));

    public IReadOnlyCollection<string> GetUserConnectionIds(string username) => OnlineUsers.Where(d => d.Username == username)
                                                                                           .Select(f => f.ConnectionId)
                                                                                            .ToList();

    internal void RemoveOnlineUserByConnectionId(string connectionId)
    { 
        var item = OnlineUsers.First(x => x.ConnectionId == connectionId);
        OnlineUsers.Remove(item);
    }

    internal IReadOnlyCollection<string> GetUsersConnectionIds(IEnumerable<string> usernames)
    {
        return OnlineUsers.Where(d => usernames.Contains(d.Username))
                          .Select(f => f.ConnectionId)
                          .ToList();
    }
}


public record OnlineUser(string Username, string ConnectionId);
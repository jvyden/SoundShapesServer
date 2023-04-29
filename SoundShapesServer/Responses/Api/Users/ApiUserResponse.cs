using Newtonsoft.Json;

namespace SoundShapesServer.Responses.Api.Users;

public class ApiUserResponse
{
    public string Id { get; set; }
    public string Username { get; set; }
    public int UserType { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int LikedLevelsCount { get; set; }
    public int PublishedLevelsCount { get; set; }
}
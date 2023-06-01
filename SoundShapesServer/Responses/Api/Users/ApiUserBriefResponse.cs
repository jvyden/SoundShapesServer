using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Responses.Api.Users;

public class ApiUserBriefResponse
{
    public ApiUserBriefResponse(GameUser user)
    {
        Id = user.Id;
        Username = user.Username;
        PermissionsType = user.PermissionsType;
        PublishedLevelsCount = user.Levels.Count();
        FollowersCount = user.Followers.Count();
    }

#pragma warning disable CS8618
    public ApiUserBriefResponse() {}
#pragma warning restore CS8618


    public string Id { get; set; }
    public string Username { get; set; }
    public int PermissionsType { get; set; }
    public int PublishedLevelsCount { get; set; }
    public int FollowersCount { get; set; }
}
using Newtonsoft.Json;
using SoundShapesServer.Helpers;
using SoundShapesServer.Responses.Game.Users;
using SoundShapesServer.Types;
using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Responses.Game.Following;

public class FollowingUserResponse
{
    public FollowingUserResponse(GameUser follower, GameUser recipient)
    {
        Id = IdFormatter.FormatFollowId(follower.Id, recipient.Id);
        User = new UserResponse(recipient);
    }

    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("type")] public string Type { get; } = GameContentType.follow.ToString();
    [JsonProperty("target")] public UserResponse User { get; set; }
}
using Newtonsoft.Json;
using SoundShapesServer.Types.Sessions;

namespace SoundShapesServer.Responses.Game.Sessions;

public class GameSessionResponse
{
    public GameSessionResponse(GameSession session)
    {
        ExpirationDate = session.ExpiresAt.ToUnixTimeMilliseconds();
        Id = session.Id;
        User = new SessionUserResponse(session.User);
    }

    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("expires")] public long? ExpirationDate { get; set; }
    [JsonProperty("person")] public SessionUserResponse? User { get; set; }
}
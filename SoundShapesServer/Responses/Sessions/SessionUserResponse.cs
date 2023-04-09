using Newtonsoft.Json;

namespace SoundShapesServer.Responses.Sessions;

public class SessionUserResponse
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("display_name")] public string DisplayName { get; set; }
}
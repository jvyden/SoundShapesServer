using Newtonsoft.Json;

namespace SoundShapesServer.Responses;

public class ServiceResponse
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("display_name")] public string DisplayName { get; set; }
}
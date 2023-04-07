using Newtonsoft.Json;

namespace SoundShapesServer.Responses.Albums;

public class AlbumResponseWrapper
{
    public AlbumResponse[] items { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)] public int? previousToken { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)] public int? nextToken { get; set; }
}
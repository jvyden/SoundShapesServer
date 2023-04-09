using Newtonsoft.Json;
using SoundShapesServer.Responses.Levels;
using SoundShapesServer.Responses.Users;
using SoundShapesServer.Types;

namespace SoundShapesServer.Responses.Albums.Levels;

public class AlbumLevelTarget
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("type")] public string Type = ResponseType.level.ToString();
    [JsonProperty("latestVersion")] public LevelVersionResponse LatestVersion { get; set; }
    [JsonProperty("author")] public UserResponse Author { get; set; }
    [JsonProperty("completed")] public bool Completed { get; set; }
    [JsonProperty("metadata")] public LevelMetadataResponse Metadata { get; set; }
}
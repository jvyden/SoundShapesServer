using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SoundShapesServer.Database;
using SoundShapesServer.Helpers;
using SoundShapesServer.Responses.Api.Levels;
using SoundShapesServer.Responses.Api.Users;
using SoundShapesServer.Types.PlayerActivity;

namespace SoundShapesServer.Responses.Api.RecentActivity;

public class ApiPlayerActivityResponse
{
    public ApiPlayerActivityResponse(GameDatabaseContext database, GameEvent eventObject)
    {
        Id = eventObject.Id;
        EventType = eventObject.EventType;
        Actor = new ApiUserResponse(eventObject.Actor);
        
        if (eventObject.ContentUser != null)
            ContentUser = new ApiUserResponse(eventObject.ContentUser);
        if (eventObject.ContentLevel != null)
            ContentLevel = new ApiLevelSummaryResponse(eventObject.ContentLevel);
        if (eventObject.ContentLeaderboardEntry != null)
            ContentLeaderboardEntry = new ApiLeaderboardEntryResponse(eventObject.ContentLeaderboardEntry, database.GetEntryPlacement(eventObject.ContentLeaderboardEntry));
        
        Date = eventObject.Date;
    }

    public string Id { get; set; }
    public int EventType { get; set; }
    public ApiUserResponse Actor { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ApiUserResponse? ContentUser { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ApiLevelSummaryResponse? ContentLevel { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ApiLeaderboardEntryResponse? ContentLeaderboardEntry { get; set; }
    public DateTimeOffset Date { get; set; }
}
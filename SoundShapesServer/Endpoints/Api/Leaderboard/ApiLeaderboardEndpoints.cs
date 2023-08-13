using AttribDoc.Attributes;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using SoundShapesServer.Database;
using SoundShapesServer.Documentation.Attributes;
using SoundShapesServer.Helpers;
using SoundShapesServer.Responses.Api;
using SoundShapesServer.Types.Leaderboard;
using SoundShapesServer.Types.Levels;
using SoundShapesServer.Types.Users;
using static SoundShapesServer.Helpers.LeaderboardHelper;

namespace SoundShapesServer.Endpoints.Api.Leaderboard;

public class ApiLeaderboardEndpoints : EndpointGroup
{
    [ApiEndpoint("leaderboard"), Authentication(false)]
    [DocUsesPageData]
    [DocSummary("Retrieves leaderboard.")]
    public ApiListResponse<ApiLeaderboardEntryResponse> GetLeaderboard(RequestContext context, GameDatabaseContext database, string id)
    {
        (int from, int count, bool descending) = PaginationHelper.GetPageData(context, false);

        string? onLevelId = context.QueryString["onLevel"];
        GameLevel? onLevel = null;
        if (onLevelId != null) onLevel = database.GetLevelWithId(onLevelId);
        
        string? byUserId = context.QueryString["byUser"];
        GameUser? byUser = null;
        if (byUserId != null) byUser = database.GetUserWithId(byUserId);
        
        
        bool onlyBest = bool.Parse(context.QueryString["onlyBest"] ?? "false");
        bool? completed = null;
        if (bool.TryParse(context.QueryString["completed"], out bool completedTemp)) completed = completedTemp;
        
        string? orderString = context.QueryString["orderBy"];

        LeaderboardOrderType order = orderString switch
        {
            "score" => LeaderboardOrderType.Score,
            "playTime" => LeaderboardOrderType.PlayTime,
            "notes" => LeaderboardOrderType.Notes,
            "creationDate" => LeaderboardOrderType.CreationDate,
            _ => LeaderboardOrderType.Score
        };

        LeaderboardFilters filters = new (onLevel, byUser, completed, onlyBest);
        (int totalEntries, LeaderboardEntry[] paginatedEntries) =
            database.GetPaginatedLeaderboardEntries(order, descending, filters, from, count);
        
        List<ApiLeaderboardEntryResponse> responses = 
            paginatedEntries.Select((e, i) => 
                new ApiLeaderboardEntryResponse(e, CalculateEntryPlacement(totalEntries, from, i, descending, true))).ToList();

        return new ApiListResponse<ApiLeaderboardEntryResponse>(responses, totalEntries);
    }
}
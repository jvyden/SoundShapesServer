using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using SoundShapesServer.Database;
using SoundShapesServer.Enums;
using SoundShapesServer.Helpers;
using SoundShapesServer.Responses;
using SoundShapesServer.Responses.Following;
using SoundShapesServer.Types;

namespace SoundShapesServer.Endpoints;

public class ProfileEndpoints : EndpointGroup
{
    [Endpoint("/otg/~identity:{id}/~metadata:*.get", ContentType.Json)]
    public ProfileMetadata ViewProfile(RequestContext context, string id, RealmDatabaseContext database)
    {
        GameUser? user = database.GetUserWithId(id);

        if (user == null) return null;

        ProfileMetadata metadata = new()
        {
            displayName = user.display_name,
            follows_of_ever_count = user.followers.Count(), // Followers
            levels_by_ever_count = user.publishedLevels.Count(), // Level Count
            follows_by_ever_count = user.following.Count(), // Following
            likes_by_ever_count = user.likedLevels.Count(), // Liked And Queued Levels
        };

        return metadata;
    }

    private FollowingUserResponsesWrapper? GetFollowUsers(RequestContext context, RealmDatabaseContext database,
        GameUser follower, IEnumerable<GameUser> followedUsers, int totalRelations, int from, int count)
    {
        (int? nextToken, int? previousToken) = PaginationHelper.GetPageTokens(totalRelations, from, count);
        
        List<GameUser> userList = followedUsers.Skip(from).Take(count).ToList();

        FollowingUserResponse[] responses = new FollowingUserResponse[userList.Count()];
        
        for (int i = 0; i < userList.Count; i++)
        {
            FollowingUserResponse response = new()
            {
                id = IdFormatter.FormatFollowId(follower.id, userList[i].id),
                target = new UserResponse()
                {
                    id = IdFormatter.FormatUserId(userList[i].id),
                    type = ResponseType.identity.ToString(),
                    displayName = userList[i].display_name
                }
            };
            
            responses[i] = response;
        }

        FollowingUserResponsesWrapper responsesWrapper = new()
        {
            items = responses,
            nextToken = nextToken,
            previousToken = previousToken
        };

        return responsesWrapper;
    }

    [Endpoint("/otg/~identity:{id}/~follow:*.page", ContentType.Json)]
    public FollowingUserResponsesWrapper? ViewFollowingList(RequestContext context, string id, RealmDatabaseContext database)
    {
        int from = int.Parse(context.QueryString["from"] ?? "0");
        int count = int.Parse(context.QueryString["count"] ?? "9");

        GameUser? follower = database.GetUserWithId(id);
        if (follower == null) return null;
        
        (GameUser[] followedUsers, int totalRelations) = database.GetFollowedUsers(follower, from, count);

        return GetFollowUsers(context, database, follower, followedUsers, totalRelations, from, count);
    }

    [Endpoint("/otg/~identity:{id}/~followers.page", ContentType.Json)]
    public FollowingUserResponsesWrapper? ViewFollowersList(RequestContext context, string id, RealmDatabaseContext database)
    {
        int from = int.Parse(context.QueryString["from"] ?? "0");
        int count = int.Parse(context.QueryString["count"] ?? "9");

        GameUser? follower = database.GetUserWithId(id);
        if (follower == null) return null;
        
        (GameUser[] followedUsers, int totalRelations) = database.GetFollowers(follower, from, count);

        return GetFollowUsers(context, database, follower, followedUsers, totalRelations, from, count);
    }
}
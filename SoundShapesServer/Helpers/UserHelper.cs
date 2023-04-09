using SoundShapesServer.Responses;
using SoundShapesServer.Responses.Following;
using SoundShapesServer.Responses.Users;
using SoundShapesServer.Types;

namespace SoundShapesServer.Helpers;

public static class UserHelper
{
    public static FollowingUsersWrapper UsersToFollowingUsersWrapper
    (GameUser user, GameUser[] users, int totalRelations, int from, int count)
    {
        (int? nextToken, int? previousToken) = PaginationHelper.GetPageTokens(totalRelations, from, count);

        List<FollowingUserResponse> responses = new ();
        
        for (int i = 0; i < users.Length; i++)
        {
            FollowingUserResponse? response = UserToFollowingUserResponse(user, users[i]);
            if (response != null) responses.Add(response);
        }

        FollowingUsersWrapper responsesWrapper = new()
        {
            Users = responses.ToArray(),
            NextToken = nextToken,
            PreviousToken = previousToken
        };

        return responsesWrapper;
    }

    private static FollowingUserResponse? UserToFollowingUserResponse(GameUser? follower, GameUser? followed)
    {
        if (follower == null || followed == null) return null;

        return new FollowingUserResponse
        {
            Id = IdFormatter.FormatFollowId(follower.Id, followed.Id),
            User = UserToUserResponse(followed)
        };
    }
    public static UserResponse? UserToUserResponse(GameUser? user)
    {
        if (user == null) return null;
        
        string formattedAuthorId = IdFormatter.FormatUserId(user.Id);

        return new UserResponse
        {
            Id = formattedAuthorId,
            Type = ResponseType.identity.ToString(),
            DisplayName = user.DisplayName
        };
    }

    public static UserMetadataResponse GenerateUserMetadata(GameUser user)
    {
        return new UserMetadataResponse()
        {
            DisplayName = user.DisplayName,
            FanCount = user.Followers.Count(), // Followers
            LevelCount = user.Levels.Count(), // Level Count
            FollowingCount = user.Following.Count(), // Following
            LikedLevelsCount = user.LikedLevels.Count(), // Liked And Queued Levels
        };
    }
}
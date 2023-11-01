using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using SoundShapesServer.Database;
using SoundShapesServer.Documentation.Attributes;
using SoundShapesServer.Extensions.Queryable;
using SoundShapesServer.Extensions.RequestContextExtensions;
using SoundShapesServer.Responses.Api.Framework;
using SoundShapesServer.Responses.Api.Framework.Errors;
using SoundShapesServer.Responses.Api.Responses.Users;
using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Endpoints.Api.Users;

public class ApiUserEndpoints : EndpointGroup
{
    [ApiEndpoint("users/id/{id}"), Authentication(false)]
    [DocSummary("Retrieves user with specified ID.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserNotFoundWhen)]
    public ApiResponse<ApiUserFullResponse> GetUserWithId(RequestContext context, GameDatabaseContext database, string id)
    {
        GameUser? userToCheck = database.GetUserWithId(id);
        if (userToCheck == null)
            return ApiNotFoundError.UserNotFound;
        
        return new ApiUserFullResponse(userToCheck);
    }
    
    [ApiEndpoint("users/username/{username}"), Authentication(false)]
    [DocSummary("Retrieves user with specified username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserNotFoundWhen)]
    public ApiResponse<ApiUserFullResponse> GetUserWithUsername(RequestContext context, GameDatabaseContext database, string username)
    {
        GameUser? userToCheck = database.GetUserWithUsername(username);
        if (userToCheck == null)
            return ApiNotFoundError.UserNotFound;
        
        return new ApiUserFullResponse(userToCheck);
    }

    [ApiEndpoint("users"), Authentication(false)]
    [DocUsesPageData]
    [DocUsesFiltration<UserFilters>]
    [DocUsesOrder<UserOrderType>]
    [DocSummary("Lists users.")]
    public ApiListResponse<ApiUserBriefResponse> GetUsers(RequestContext context, GameDatabaseContext database)
    {
        (int from, int count, bool descending) = context.GetPageData();

        UserFilters filters = context.GetFilters<UserFilters>(database);
        UserOrderType order = context.GetOrderType<UserOrderType>() ?? UserOrderType.CreationDate;

        (GameUser[] users, int totalUsers) = database.GetPaginatedUsers(order, descending, filters, from, count);
        return new ApiListResponse<ApiUserBriefResponse>(users.Select(u=>new ApiUserBriefResponse(u)), totalUsers);
    }
}
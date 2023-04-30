using System.Net;
using System.Reflection.Metadata.Ecma335;
using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using Bunkum.HttpServer.Responses;
using Bunkum.HttpServer.Storage;
using SoundShapesServer.Database;
using SoundShapesServer.Helpers;
using SoundShapesServer.Requests.Api;
using SoundShapesServer.Types;

namespace SoundShapesServer.Endpoints.Api.Moderation;

public class ApiManageUserEndpoints : EndpointGroup
{
    [ApiEndpoint("user/{id}/remove", Method.Post)]
    public Response RemoveUser(RequestContext context, RealmDatabaseContext database, IDataStore dataStore, GameUser user, string id)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Forbidden;

        GameUser? userToRemove = database.GetUserWithId(id);
        if (userToRemove == null) return HttpStatusCode.NotFound;

        if (userToRemove.PermissionsType >= user.PermissionsType) return HttpStatusCode.Unauthorized;
        
        database.RemoveUser(userToRemove, dataStore);
        return HttpStatusCode.OK;
    }

    [ApiEndpoint("user/{id}/setPermissions", Method.Post)]
    public Response SetUserPermissions(RequestContext context, RealmDatabaseContext database, GameUser user, string id, ApiSetUserPermissionsRequest body)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Forbidden;

        GameUser? userToSetPermissionsOf = database.GetUserWithId(id);
        if (userToSetPermissionsOf == null) return HttpStatusCode.Forbidden;

        if (Enum.TryParse(body.PermissionsType.ToString(), out PermissionsType type) == false) return HttpStatusCode.BadRequest;
        if (body.PermissionsType > user.PermissionsType || userToSetPermissionsOf.PermissionsType >= user.PermissionsType) return HttpStatusCode.Unauthorized;
        
        
        database.SetUserPermissions(userToSetPermissionsOf, type);
        return HttpStatusCode.OK;
    }
}
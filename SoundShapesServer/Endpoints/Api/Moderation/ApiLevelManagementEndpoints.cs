using System.Net;
using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using Bunkum.HttpServer.Responses;
using Bunkum.HttpServer.Storage;
using SoundShapesServer.Database;
using SoundShapesServer.Helpers;
using SoundShapesServer.Requests.Api;
using SoundShapesServer.Requests.Game;
using SoundShapesServer.Responses.Api.Levels;
using SoundShapesServer.Types;
using SoundShapesServer.Types.Levels;
using SoundShapesServer.Types.Users;
using static SoundShapesServer.Helpers.ResourceHelper;

namespace SoundShapesServer.Endpoints.Api.Moderation;

public class ApiLevelManagementEndpoints : EndpointGroup
{
    // TODO: THIS SHOULD NOT BE AN ENDPOINT. GET IT OUT OF HERE AND MAKE IT RUN ONLY ON BOOT
    [ApiEndpoint("levels/import", Method.Post)]
    public Response RefreshImportFolder(RequestContext context, GameDatabaseContext database, IDataStore dataStore, GameUser user)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Unauthorized;

        LevelImporting.ImportLevels(database, dataStore);
        return HttpStatusCode.Created;
    }
    
    [ApiEndpoint("levels/create", Method.Post)]
    public Response CreateLevel(RequestContext context, GameDatabaseContext database, GameUser user, ApiPublishLevelRequest body)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Unauthorized;

        GameLevel publishedLevel = database.CreateLevel(new PublishLevelRequest(body), user);
        return new Response(new ApiLevelFullResponse(publishedLevel), ContentType.Json, HttpStatusCode.Created);
    }

    [ApiEndpoint("levels/{id}/setLevel", Method.Post)]
    public Response UploadLevelFile
    (RequestContext context, GameDatabaseContext database, IDataStore dataStore, GameUser user, Stream body,
        string id)
        => UploadLevelResources(database, dataStore, user, body, id, FileType.Level);
    
    [ApiEndpoint("levels/{id}/setSound", Method.Post)]
    public Response UploadSoundFile
    (RequestContext context, GameDatabaseContext database, IDataStore dataStore, GameUser user, Stream body,
        string id)
        => UploadLevelResources(database, dataStore, user, body, id, FileType.Sound);
    
    [ApiEndpoint("levels/{id}/setThumbnail", Method.Post)]
    public Response UploadThumbnail
    (RequestContext context, GameDatabaseContext database, IDataStore dataStore, GameUser user, Stream body,
        string id)
        => UploadLevelResources(database, dataStore, user, body, id, FileType.Image);

    private Response UploadLevelResources(GameDatabaseContext database, IDataStore dataStore, GameUser user,
        Stream body, string id, FileType fileType)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Unauthorized;

        GameLevel? level = database.GetLevelWithId(id);
        if (level == null) return HttpStatusCode.NotFound;

        byte[] file;

        using (MemoryStream memoryStream = new())
        {
            body.CopyTo(memoryStream);
            file = memoryStream.ToArray();
        }

        if (fileType == FileType.Image && !IsByteArrayPng(file))
            return new Response("Image is not a PNG.", ContentType.Plaintext, HttpStatusCode.BadRequest);

        string key = GetLevelResourceKey(id, fileType);
        dataStore.WriteToStore(key, file);
        if (fileType == FileType.Level) database.SetLevelFileSize(level, file.Length);

        return HttpStatusCode.Created;
    }

    // Remove Levels is in ../Levels/ApiLevelEndpoints
}
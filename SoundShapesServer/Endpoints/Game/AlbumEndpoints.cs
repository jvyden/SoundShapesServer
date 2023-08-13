using System.Net;
using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using Bunkum.HttpServer.Responses;
using SoundShapesServer.Database;
using SoundShapesServer.Helpers;
using SoundShapesServer.Responses.Game;
using SoundShapesServer.Responses.Game.Albums;
using SoundShapesServer.Responses.Game.Albums.LevelInfo;
using SoundShapesServer.Responses.Game.Levels;
using SoundShapesServer.Types.Albums;
using SoundShapesServer.Types.Levels;
using SoundShapesServer.Types.Sessions;
using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Endpoints.Game;

public class AlbumEndpoints : EndpointGroup
{
    [GameEndpoint("~albums/~link:*.page")]
    public ListResponse<AlbumResponse> GetAlbums(RequestContext context, GameDatabaseContext database, GameSession session)
    {
        (int from, int count, bool _) = PaginationHelper.GetPageData(context);

        (GameAlbum[] albums, int totalAlbums) = database.GetPaginatedAlbums(AlbumOrderType.CreationDate, true, from, count);

        return new ListResponse<AlbumResponse>(albums.Select(a => new AlbumResponse(a)), totalAlbums, from, count);
    }

    [GameEndpoint("~album:{albumId}/~link:*.page")]
    public Response GetAlbumLevels
        (RequestContext context, GameDatabaseContext database, GameUser user, string albumId)
    {
        (int from, int count, bool _) = PaginationHelper.GetPageData(context);
        string? order = context.QueryString["order"];

        GameAlbum? album = database.GetAlbumWithId(albumId);

        if (album == null) return HttpStatusCode.NotFound;

        (GameLevel[] levels, int totalLevels) = database.GetPaginatedLevels(LevelOrderType.Difficulty, true, new LevelFilters(inAlbum: album), from, count, user);

        if (order == "time:ascn")
            return new Response(new ListResponse<LevelResponse>(levels.Select(l=>new LevelResponse(l, user)), totalLevels, from, count), ContentType.Json);

        return new Response(new ListResponse<AlbumLevelInfoResponse>(levels.Select(l => new AlbumLevelInfoResponse(l, album, user)), totalLevels, from, count), ContentType.Json);
    }
    
    [GameEndpoint("{platform}/{publisher}/{language}/~translation.get")]
    public Response GetTranslatedLinerNotes(RequestContext context, string platform, string publisher, string language)
    {
        // This is for Album Translations
        return new Response(HttpStatusCode.OK);
    }
}
using System.Net;
using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Responses;
using Bunkum.HttpServer.Storage;
using HttpMultipartParser;
using SoundShapesServer.Database;
using SoundShapesServer.Helpers;
using SoundShapesServer.Types;
using static SoundShapesServer.Helpers.ResourceHelper;

namespace SoundShapesServer.Endpoints.Api.Moderation;

public class ApiManageNewsEndpoints
{
    [ApiEndpoint("news/create", Method.Post)]
    public Response CreateNewsEntry(RequestContext context, RealmDatabaseContext database, IDataStore dataStore, Stream body, GameUser user)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Forbidden;
        
        MultipartFormDataParser request = MultipartFormDataParser.Parse(body);

        string language;
        string title;
        string summary;
        string fullText;
        string url;

        byte[] image;
        
        try
        {
            language = request.Parameters.First(p => p.Name == "Language").Data;
            title = request.Parameters.First(p => p.Name == "Title").Data;
            summary = request.Parameters.First(p => p.Name == "Summary").Data;
            fullText = request.Parameters.First(p => p.Name == "FullText").Data;
            url = request.Parameters.First(p => p.Name == "Url").Data;

            image = FilePartToBytes(request.Files.First(p => p.Name == "Image"));
        }
        catch (InvalidOperationException)
        {
            return HttpStatusCode.BadRequest;
        }
        
        if (!IsByteArrayPng(image)) return new Response("Image is not a PNG.", ContentType.Plaintext, HttpStatusCode.BadRequest);

        dataStore.WriteToStore(GetNewsResourceKey(language), image);
        database.CreateNewsEntry(new NewsEntry()
        {
            Language = language, 
            Title = title, 
            Summary = summary, 
            FullText = fullText, 
            Url = url
        });

        return HttpStatusCode.Created;
    }

    [ApiEndpoint("news/{language}/delete", Method.Post)]
    public Response DeleteNewsEntry(RequestContext context, RealmDatabaseContext database, GameUser user, string language)
    {
        if (PermissionHelper.IsUserAdmin(user) == false) return HttpStatusCode.Forbidden;

        NewsEntry? newsEntry = database.GetNews(language);
        if (newsEntry == null) return HttpStatusCode.NotFound;
        
        database.RemoveNewsEntry(newsEntry);
        return HttpStatusCode.OK;
    }
}
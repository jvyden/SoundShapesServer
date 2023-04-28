using System.Net;
using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using Bunkum.HttpServer.Responses;
using SoundShapesServer.Database;
using SoundShapesServer.Requests.Api;
using SoundShapesServer.Responses.Api;
using SoundShapesServer.Types;

namespace SoundShapesServer.Endpoints.Api;

public class ApiIpAuthorization : EndpointGroup
{
    [ApiEndpoint("ip/authorize", Method.Post)]
    public Response AuthorizeIpAddress(RequestContext context, RealmDatabaseContext database, ApiAuthorizeIpRequest body, GameUser user)
    {
        IpAuthorization ip = database.GetIpFromAddress(user, body.IpAddress, (int)SessionType.Game);

        if (database.AuthorizeIpAddress(ip, body.OneTimeUse))
            return HttpStatusCode.Created;

        return HttpStatusCode.Conflict;
    }
    [ApiEndpoint("ip/unAuthorize", Method.Post)]
    public Response UnAuthorizeIpAddress(RequestContext context, RealmDatabaseContext database, ApiUnAuthorizeIpRequest body, GameUser user)
    {
        IpAuthorization ip = database.GetIpFromAddress(user, body.IpAddress, (int)SessionType.Game);

        database.RemoveIpAddress(ip);
        return HttpStatusCode.OK;
    }

    [ApiEndpoint("ip/unAuthorized")]
    public ApiUnAuthorizedIpResponseWrapper UnAuthorizedIps(RequestContext context, RealmDatabaseContext database, GameUser user)
    {
        ApiUnAuthorizedIpResponse[] addresses = database.GetUnAuthorizedIps(user, SessionType.Game);

        return new ApiUnAuthorizedIpResponseWrapper()
        {
            IpAddresses = addresses
        };
    }

    [ApiEndpoint("ip/authorized")]
    public ApiAuthorizedIpResponseWrapper AuthorizedIps(RequestContext context, RealmDatabaseContext database, GameUser user)
    {
        ApiAuthorizedIpResponse[] addresses = database.GetAuthorizedIps(user, SessionType.Game);

        return new ApiAuthorizedIpResponseWrapper
        {
            IpAddresses = addresses
        };
    }
}
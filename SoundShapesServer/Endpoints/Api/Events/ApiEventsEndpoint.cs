using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using SoundShapesServer.Database;
using SoundShapesServer.Documentation.Attributes;
using SoundShapesServer.Extensions;
using SoundShapesServer.Helpers;
using SoundShapesServer.Responses.Api.Framework;
using SoundShapesServer.Responses.Api.Responses;
using SoundShapesServer.Types.Events;
using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Endpoints.Api.Events;

public class ApiEventsEndpoint : EndpointGroup
{
    [ApiEndpoint("events"), Authentication(false)]
    [DocUsesPageData]
    [DocSummary("Lists events.")]
    public ApiListResponse<ApiEventResponse> GetEvents(RequestContext context, GameDatabaseContext database, GameUser? user)
    {
        (int from, int count, bool descending) = context.GetPageData();

        EventFilters filters = EventHelper.GetEventFilters(context, database);
        EventOrderType orderType = EventHelper.GetEventOrder(context);
        
        (GameEvent[] events, int totalEvents) = database.GetPaginatedEvents(orderType, descending, filters, from, count, user);
        
        return new ApiListResponse<ApiEventResponse>(events.Select(e=>new ApiEventResponse(database, e)), totalEvents);
    }
}
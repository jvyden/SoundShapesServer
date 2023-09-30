using SoundShapesServer.Responses.Api.Framework;

namespace SoundShapesServer.Responses.Api.Responses.Levels;

public class ApiLevelRelationResponse : IApiResponse
{
    public bool Completed { get; set; }
    public bool Liked { get; set; }
    public bool Queued { get; set; }
}
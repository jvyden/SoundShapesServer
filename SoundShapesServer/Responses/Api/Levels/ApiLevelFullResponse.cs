using SoundShapesServer.Responses.Api.Users;
using SoundShapesServer.Types.Levels;

namespace SoundShapesServer.Responses.Api.Levels;

public class ApiLevelFullResponse
{
    public ApiLevelFullResponse(GameLevel level)
    {
        Id = level.Id;
        Name = level.Name;
        Author = new ApiUserBriefResponse(level.Author);
        CreationDate = level.CreationDate;
        ModificationDate = level.ModificationDate;
        TotalPlays = level.PlaysCount;
        UniquePlays = level.UniquePlaysCount;
        Likes = level.Likes.Count();
        Deaths = level.Deaths;
        TotalPlayTime = level.TotalPlayTime;
        Language = level.Language;
        Difficulty = level.Difficulty;
        AlbumIds = level.Albums.AsEnumerable().Select(a => a.Id).ToArray();
        DailyLevelIds = level.DailyLevels.AsEnumerable().Select(d => d.Id).ToArray();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public ApiUserBriefResponse Author { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
    public int TotalPlays { get; set; }
    public int UniquePlays { get; set; }
    public int Likes { get; set; }
    public int Deaths { get; set; }
    public long TotalPlayTime { get; set; }
    public int Language { get; set; }
    public float Difficulty { get; set; }
    public string[] AlbumIds { get; set; }
    public string[] DailyLevelIds { get; set; }
}
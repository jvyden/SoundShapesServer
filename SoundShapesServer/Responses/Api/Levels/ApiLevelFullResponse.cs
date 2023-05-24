using SoundShapesServer.Responses.Api.Albums;
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
        TotalCompletions = level.CompletionCount;
        UniqueCompletions = level.UniqueCompletions.Count;
        Likes = level.Likes.Count();
        TotalDeaths = level.TotalDeaths;
        TotalPlayTime = level.TotalPlayTime;
        Language = level.Language;
        Difficulty = level.Difficulty;
        FileSize = level.FileSize;
        Bpm = level.Bpm;
        TransposeValue = level.TransposeValue;
        ScaleIndex = level.ScaleIndex;
        TotalScreens = level.TotalScreens;
        TotalEntities = level.TotalEntities;
        HasCar = level.HasCar;
        HasExplodingCar = level.HasExplodingCar;
        Albums = level.Albums.AsEnumerable().Select(a => new ApiAlbumResponse(a)).ToArray();
        DailyLevels = level.DailyLevels.AsEnumerable().Select(a => new ApiDailyLevelResponse(a)).ToArray();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public ApiUserBriefResponse Author { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
    public int TotalPlays { get; set; }
    public int UniquePlays { get; set; }
    public int TotalCompletions { get; set; }
    public int UniqueCompletions { get; set; }
    public int Likes { get; set; }
    public int TotalDeaths { get; set; }
    public long TotalPlayTime { get; set; }
    public int Language { get; set; }
    public float Difficulty { get; set; }
    public long FileSize { get; set; }
    public int Bpm { get; set; }
    public int TransposeValue { get; set; }
    public int ScaleIndex { get; set; }
    public int TotalScreens { get; set; }
    public int TotalEntities { get; set; }
    public bool HasCar { get; set; }
    public bool HasExplodingCar { get; set; }
    public ApiAlbumResponse[] Albums { get; set; }
    public ApiDailyLevelResponse[] DailyLevels { get; set; }
}
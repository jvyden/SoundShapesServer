using Realms;
using SoundShapesServer.Types.Levels;
using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Types.Relations;

public class LevelLikeRelation : RealmObject
{
    public LevelLikeRelation(DateTimeOffset date, GameUser liker, GameLevel level)
    {
        Date = date;
        Liker = liker;
        Level = level;
    }
    
    // Realm cries if this doesn't exist
    #pragma warning disable CS8618
    public LevelLikeRelation() {}
    #pragma warning restore CS8618

    public DateTimeOffset Date { get; set; }
    public GameUser Liker { get; init; }
    public GameLevel Level { get; init; }
}
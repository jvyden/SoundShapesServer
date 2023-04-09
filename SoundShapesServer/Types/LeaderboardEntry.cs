using MongoDB.Bson;
using Realms;

namespace SoundShapesServer.Types;

public class LeaderboardEntry : RealmObject
{
    public ObjectId Id = ObjectId.GenerateNewId();
    public GameUser User { get; set; }
    public string LevelId { get; set; }
    public long Score { get; set; }
    public int Playtime { get; set; }
    public int Deaths { get; set; }
    public int Golded { get; set; }
    public int TokenCount { get; set; }
    public bool Completed { get; set; }
    public DateTimeOffset Date { get; set; }
}
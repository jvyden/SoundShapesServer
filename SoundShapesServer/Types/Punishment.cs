using MongoDB.Bson;
using Realms;

namespace SoundShapesServer.Types;

public class Punishment : RealmObject
{
    public ObjectId Id = ObjectId.GenerateNewId();
    public GameUser User { get; set; }
    public int PunishmentType { get; set; }
    public string Reason { get; set; }
    public bool Revoked { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
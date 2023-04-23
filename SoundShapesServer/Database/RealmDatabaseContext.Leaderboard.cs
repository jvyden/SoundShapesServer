using SoundShapesServer.Requests.Game;
using SoundShapesServer.Types;

namespace SoundShapesServer.Database;

public partial class RealmDatabaseContext
{
    public bool SubmitScore(LeaderboardSubmissionRequest request, GameUser user, string levelId)
    {
        LeaderboardEntry entry = new()
        {
            User = user,
            LevelId = levelId,
            Score = request.Score,
            Playtime = request.PlayTime,
            Deaths = request.Deaths,
            Golded = request.Golded,
            TokenCount = request.TokenCount,
            Completed = Convert.ToBoolean(request.Completed),
            Date = DateTimeOffset.UtcNow
        };

        LeaderboardEntry? previousEntry =
            this._realm.All<LeaderboardEntry>().FirstOrDefault(e => e.LevelId == levelId && e.User == user && e.Completed);

        this._realm.Write(() =>
        {
            // If there is a previous entry, and it's more than the new one, remove it and replace it with the new one
            if (previousEntry != null && previousEntry.Score > entry.Score)
            {
                this._realm.Remove(previousEntry);
                this._realm.Add(entry);
            }
            else if (previousEntry == null)
            {
                this._realm.Add(entry);
            }
            else
            {
                Console.WriteLine("not submitting score");
            }
        });

        return true;
    }

    public (LeaderboardEntry[], int) GetLeaderboardEntries(string levelId, int from, int count)
    {
        IEnumerable<LeaderboardEntry> entries = this._realm.All<LeaderboardEntry>()
            .AsEnumerable()
            .Where(e=>e.Completed)
            .Where(e => e.LevelId == levelId);

        int totalEntries = entries.Count();

        IEnumerable<LeaderboardEntry> selectedEntries = entries
            .OrderBy(e => e.Score)
            .Skip(from)
            .Take(count);

        return (selectedEntries.ToArray(), totalEntries);
    }

    public LeaderboardEntry? GetLeaderboardEntryFromPlayer(GameUser user, string levelId)
    {
        return this._realm.All<LeaderboardEntry>().Where(e=>e.Completed).FirstOrDefault(e => e.User == user && e.LevelId == levelId);
    }

    public int GetPositionOfLeaderboardEntry(LeaderboardEntry entry)
    {
        return this._realm.All<LeaderboardEntry>().Count(e => e.LevelId == entry.LevelId && e.Score < entry.Score && e.Completed) + 1;
    }
}
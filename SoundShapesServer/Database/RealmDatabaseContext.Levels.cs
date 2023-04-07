using Realms;
using Realms.Sync;
using SoundShapesServer.Enums;
using SoundShapesServer.Helpers;
using SoundShapesServer.Requests;
using SoundShapesServer.Responses.Levels;
using SoundShapesServer.Types;
using SoundShapesServer.Types.Levels;

namespace SoundShapesServer.Database;

public partial class RealmDatabaseContext
{
    public LevelPublishResponse PublishLevel(LevelPublishRequest request, GameUser user)
    {
        string levelId = request.levelId;
            
        GameLevel level = new GameLevel
        {
            id = levelId,
            author = user,
            title = request.title,
            description = request.description,
            visibility = Visibility.EVERYONE.ToString(),
            scp_np_language = request.sce_np_language,
            created = DateTimeOffset.UtcNow,
            modified = DateTimeOffset.UtcNow
        };

        this._realm.Write(() =>
        {
            this._realm.Add(level);
        });

        return GeneratePublishResponse(level);
    }

    public LevelPublishResponse? UpdateLevel(LevelPublishRequest updatedLevel, GameLevel level, GameUser user)
    {
        if (!user.Equals(level.author)) return null;
        
        this._realm.Write(() =>
        {
            level.title = updatedLevel.title;
            level.description = updatedLevel.description;
            level.scp_np_language = updatedLevel.sce_np_language;
            level.modified = DateTimeOffset.UtcNow;
        });

        return GeneratePublishResponse(level);
    }

    private const string LevelIdCharacters = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int LevelIdLength = 8;
    
    public string GenerateLevelId()
    {
        Random r = new Random();
        string levelId = "";
        for (int i = 0; i < LevelIdLength; i++)
        {
            levelId += LevelIdCharacters[r.Next(LevelIdCharacters.Length - 1)];
        }

        if (GetLevelWithId(levelId) == null) return levelId; // Return if LevelId has not been used before
        return GenerateLevelId(); // Generate new LevelId if it already exists
    }

    private LevelPublishResponse GeneratePublishResponse(GameLevel level)
    {
        return new () {
            id = IdFormatter.FormatLevelPublishId(level.id, level.created.ToUnixTimeMilliseconds()),
            type = ResponseType.upload.ToString(),
            author = new()
            {
                id = IdFormatter.FormatUserId(level.author.id),
                type = ResponseType.identity.ToString(),
                displayName = level.author.display_name
            },
            title = level.title,
            dependencies = new List<string>(),
            visibility = level.visibility,
            description = level.description,
            extraData = new ExtraData() { sce_np_language = level.scp_np_language },
            parent = new()
            {
                id = IdFormatter.FormatLevelId(level.id),
                type = ResponseType.level.ToString()
            },
            creationTime = level.created.ToUnixTimeMilliseconds()
        };   
    }

    public bool UnPublishLevel(GameLevel level)
    {
        this._realm.Write(() =>
        {
            this._realm.Remove(level);
        });

        return true;
    }
    
    public GameLevel? GetLevelWithId(string id) => this._realm.All<GameLevel>().FirstOrDefault(l => l.id == id);
    public (GameLevel[], int) GetLevelsPublishedByUser(GameUser user, int from, int count)
    {
        IQueryable<GameLevel> entries = this._realm.All<GameLevel>()
            .Where(l => l.author == user);

        int totalEntries = entries.Count();
        
        
        GameLevel[] selectedEntries = entries
            .AsEnumerable()
            .Skip(from)
            .Take(count)
            .ToArray();

        return (selectedEntries, totalEntries);
    }

    public (GameLevel[], int) SearchForLevels(string query, int from, int count)
    {
        string[] keywords = query.Split(' ');
        if (keywords.Length == 0) return (Array.Empty<GameLevel>(), 0);
        
        IQueryable<GameLevel> entries = this._realm.All<GameLevel>();
        
        foreach (string keyword in keywords)
        {
            if (string.IsNullOrWhiteSpace(keyword)) continue;

            entries = entries.Where(l =>
                l.title.Like(keyword, false)
            );
        }

        int totalEntries = entries.Count();
        
        GameLevel[] selectedEntries = entries
            .AsEnumerable()
            .Skip(from)
            .Take(count)
            .ToArray();

        return (selectedEntries, totalEntries);
    }

    public bool AddCompletionToLevel(GameLevel level)
    {
        this._realm.Write((() =>
        {
            level.completitions++;
        }));

        return true;
    }
    public bool AddPlayToLevel(GameLevel level)
    {
        this._realm.Write((() =>
        {
            level.plays++;
        }));

        return true;
    }
    public bool AddUniquePlayToLevel(GameLevel level, GameUser user)
    {
        if (level.uniquePlays.Contains(user)) return false;
        
        this._realm.Write((() =>
        {
            level.uniquePlays.Add(user);
        }));

        return true;
    }

    public bool AddDeathsToLevel(GameLevel level, int deaths)
    {
        this._realm.Write((() =>
        {
            level.deaths += deaths;
        }));

        return true;
    }
}
using Bunkum.CustomHttpListener.Listeners.Direct;
using SoundShapesServer.Database;
using SoundShapesServer.Requests.Game;
using SoundShapesServer.Types;
using SoundShapesServer.Types.Leaderboard;
using SoundShapesServer.Types.Levels;
using SoundShapesServer.Types.Sessions;
using SoundShapesServer.Types.Users;
using SoundShapesServerTests.Server;

namespace SoundShapesServerTests;

public class TestContext : IDisposable
{
    public Lazy<TestServer> Server { get; }
    public GameDatabaseContext Database { get; }
    public HttpClient Http { get; }
    private DirectHttpListener Listener { get; }
    
    public TestContext(Lazy<TestServer> server, GameDatabaseContext database, HttpClient http, DirectHttpListener listener)
    {
        Server = server;
        Database = database;
        Http = http;
        Listener = listener;
    }
    
    private int _users;
    private int UserIncrement => _users++;

    public HttpClient GetAuthenticatedClient(SessionType type,
        GameUser? user = null,
        int tokenExpirySeconds = GameDatabaseContext.DefaultSessionExpirySeconds)
    {
        return GetAuthenticatedClient(type, out _, user, tokenExpirySeconds);
    }
    
    public HttpClient GetAuthenticatedClient(SessionType type, out string sessionId,
        GameUser? user = null,
        int tokenExpirySeconds = GameDatabaseContext.DefaultSessionExpirySeconds)
    {
        user ??= CreateUser();

        PlatformType? platformType = type switch
        {
            SessionType.Game => PlatformType.PsVita,
            _ => null
        };

        GameSession session = Database.CreateSession(user, type, tokenExpirySeconds, platformType:platformType);
        sessionId = session.Id;
        
        HttpClient client = Listener.GetClient();

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (type is SessionType.Game or SessionType.GameBanned or SessionType.GameUnAuthorized)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-OTG-Identity-SessionId", session.Id);
        }
        else
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", session.Id);
        }

        return client;
    }

    public GameUser CreateUser(string? username = null)
    {
        username ??= UserIncrement.ToString();
        return Database.CreateUser(username);
    }
    
    public GameLevel CreateLevel(GameUser author, string title = "Level")
    {
        PublishLevelRequest request = new (title, 0);
        return Database.CreateLevel(author, request);
    }
    
    public void FillLeaderboard(GameLevel level, int amount)
    {
        for (int i = amount; i > 0; i--)
        {
            GameUser scoreUser = Database.CreateUser("score" + i);
            SubmitLeaderboardEntry(i, level, scoreUser);
        }
    }
    
    public LeaderboardEntry SubmitLeaderboardEntry(int score, GameLevel level, GameUser user)
    {
        LeaderboardSubmissionRequest request = new()
        {
            Score = score
        };

        LeaderboardEntry entry = Database.CreateLeaderboardEntry(request, user, level.Id);

        return entry;
    }

    public void Dispose()
    {
        Database.Dispose();
        Http.Dispose();
        Listener.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
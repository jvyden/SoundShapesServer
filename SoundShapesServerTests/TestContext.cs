using Bunkum.CustomHttpListener.Listeners.Direct;
using SoundShapesServer.Database;
using SoundShapesServer.Types;
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

        GameSession session = this.Database.CreateSession(user, type, tokenExpirySeconds, platformType:platformType);
        sessionId = session.Id;
        
        HttpClient client = this.Listener.GetClient();

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

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
﻿using System.Reflection;
using Bunkum.HttpServer;
using Bunkum.HttpServer.RateLimit;
using Bunkum.HttpServer.Storage;
using SoundShapesServer;
using SoundShapesServer.Authentication;
using SoundShapesServer.Configuration;
using SoundShapesServer.Database;
using SoundShapesServer.Helpers;
using SoundShapesServer.Middlewares;
using SoundShapesServer.Services;
using SoundShapesServer.Types.Users;

using GameDatabaseProvider databaseProvider = new();

FileSystemDataStore dataStore = new ();
databaseProvider.Initialize();
GameDatabaseContext databaseContext = databaseProvider.GetContext();

#region Admin User Credentials

GameUser adminUser = databaseContext.GetAdminUser();
if (string.IsNullOrEmpty(adminUser.Email))
{
    Console.WriteLine("Admin user does not have an assigned email address.");
    Console.WriteLine("Enter an email address for the Admin user.");
    string? input = Console.ReadLine();
    if (input != null)
    {
        databaseContext.SetUserEmail(adminUser, input);
        Console.WriteLine($"Admin user's email has been set to {input}");
    }
}
if (string.IsNullOrEmpty(adminUser.PasswordBcrypt))
{
    Console.WriteLine("Admin user does not have an assigned password.");
    Console.WriteLine("Enter a password for the Admin user.");
    string? input = Console.ReadLine();
    if (input != null)
    {
        string hashedPassword = UserHelper.HashString(input);
        databaseContext.SetUserPassword(adminUser, hashedPassword);
        Console.WriteLine($"Admin user's password has been set to {input}");
    }
}

#endregion

LevelImporting.ImportLevels(databaseContext, dataStore);

BunkumHttpServer server = new();

server.DiscoverEndpointsFromAssembly(Assembly.GetExecutingAssembly());

server.UseJsonConfig<GameServerConfig>("gameServer.json");

server.UseDatabaseProvider(databaseProvider);
server.AddStorageService(dataStore);
server.AddAuthenticationService(new SessionProvider(), true);
server.AddRateLimitService(new RateLimitSettings(60, 400, 60)); // Todo: figure out a good balance here between security and usability
server.AddService<EmailService>();

server.AddMiddleware<CrossOriginMiddleware>();
server.AddMiddleware<FileSizeMiddleware>();

await server.StartAndBlockAsync();
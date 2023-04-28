using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Bunkum.CustomHttpListener.Parsing;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Endpoints;
using Bunkum.HttpServer.Responses;
using SoundShapesServer.Authentication;
using SoundShapesServer.Database;
using SoundShapesServer.Requests.Api;
using SoundShapesServer.Responses.Api;
using SoundShapesServer.Services;
using SoundShapesServer.Types;
using SoundShapesServer.Types.Levels;
using static SoundShapesServer.Helpers.SessionHelper;

namespace SoundShapesServer.Endpoints.Api;

public partial class ApiAuthenticationEndpoints : EndpointGroup
{
    [GeneratedRegex("^[a-f0-9]{128}$")]
    private static partial Regex Sha512Regex();
    
    private const int WorkFactor = 10;

    [ApiEndpoint("login", Method.Post)]
    [Authentication(false)]
    public Response Login(RequestContext context, RealmDatabaseContext database, ApiLoginRequest body)
    {
        GameUser? user = database.GetUserWithEmail(body.Email);
        if (user == null)
        {
            return new Response(new ApiErrorResponse {Reason = "The email address or password was incorrect."}, ContentType.Json, HttpStatusCode.Forbidden);
        }

        if (BCrypt.Net.BCrypt.PasswordNeedsRehash(user.PasswordBcrypt, WorkFactor))
        {
            database.SetUserPassword(user, BCrypt.Net.BCrypt.HashPassword(body.PasswordSha512, WorkFactor));
        }

        if (BCrypt.Net.BCrypt.Verify(body.PasswordSha512, user.PasswordBcrypt) == false)
        {
            return new Response(new ApiErrorResponse {Reason = "The email address or password was incorrect."}, ContentType.Json, HttpStatusCode.Forbidden);
        }
        
        GameSession session = database.GenerateSessionForUser(context, user, SessionType.API);

        ApiAuthenticationResponse response = new()
        {
            Id = session.Id,
            ExpiresAt = session.ExpiresAt,
            UserId = user.Id,
            Username = user.Username
        };

        return new Response(response, ContentType.Json);
    }

    [ApiEndpoint("setEmail", Method.Post)]
    public Response SetUserEmail(RequestContext context, RealmDatabaseContext database, ApiSetEmailRequest body, GameSession session)
    {
        if (session.SessionType != (int)SessionType.SetEmail) return HttpStatusCode.Unauthorized;

        GameUser user = session.User;

        if (user.HasFinishedRegistration)
        {
            return new Response(new ApiErrorResponse {Reason = "User has already finished registration."}, ContentType.Json, HttpStatusCode.Conflict);
        }

        // Check if user has sent a valid mail address
        if (MailAddress.TryCreate(body.Email, out MailAddress? mailAddress) == false)
        {
            return new Response(new ApiErrorResponse {Reason = "Invalid Email."}, ContentType.Json, HttpStatusCode.BadRequest);
        }
        
        // Check if mail address has been used before
        GameUser? userWithEmail = database.GetUserWithEmail(body.Email);
        if (userWithEmail != null && !userWithEmail.Equals(user)) return new Response(new ApiErrorResponse {Reason = "Email already taken."}, ContentType.Json, HttpStatusCode.Forbidden);
        
        database.SetUserEmail(user, body.Email, session);

        return SendPasswordSession(context, database, new ApiGetPasswordSessionRequest { Email = body.Email });
    }
    
    [ApiEndpoint("setPassword", Method.Post)]
    public Response SetUserPassword(RequestContext context, RealmDatabaseContext database, ApiSetPasswordRequest body, GameSession session)
    {
        if (session.SessionType != (int)SessionType.SetPassword) return HttpStatusCode.Unauthorized;

        GameUser user = session.User;

        if (body.PasswordSha512.Length != 128 || !Sha512Regex().IsMatch(body.PasswordSha512))
            return new Response("Password is definitely not SHA512. Please hash the password.",
                ContentType.Plaintext, HttpStatusCode.BadRequest);

        string passwordBcrypt = BCrypt.Net.BCrypt.HashPassword(body.PasswordSha512, WorkFactor);
        
        database.SetUserPassword(user, passwordBcrypt, session);

        return HttpStatusCode.Created;
    }

    [ApiEndpoint("sendPasswordSession", Method.Post)]
    [Authentication(false)]
    public Response SendPasswordSession(RequestContext context, RealmDatabaseContext database, ApiGetPasswordSessionRequest body)
    {
        GameUser? user = database.GetUserWithEmail(body.Email);

        if (user == null) return HttpStatusCode.Created; // trol
        
        string passwordSessionId = GenerateSimpleSessionId(database, "ABCDEFGHIJKLMNOPQRSTUVWXYZ", 8);
        GameSession passwordSession = database.GenerateSessionForUser(context, user, SessionType.SetPassword, 600, passwordSessionId); // 10 minutes

        string emailBody = $"Dear {user.Username},\n\n" +
                           "Here is your password code: " + passwordSession.Id + "\n" +
                           "If this wasn't you, feel free to ignore this email.";

        EmailService? emailService = context.Services.OfType<EmailService>().FirstOrDefault();
        emailService?.SendEmail(body.Email, "Sound Shapes Password Code", emailBody);

        return HttpStatusCode.Created;
    }

    [ApiEndpoint("logout", Method.Post)]
    public Response Logout(RequestContext context, RealmDatabaseContext database, GameSession session)
    {
        database.RemoveSession(session);
        return HttpStatusCode.OK;
    }
}
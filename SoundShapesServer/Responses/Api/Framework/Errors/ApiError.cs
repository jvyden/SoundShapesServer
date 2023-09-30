﻿using System.Net;
using Bunkum.CustomHttpListener.Parsing;

namespace SoundShapesServer.Responses.Api.Framework.Errors;

public class ApiError
{
    public ApiError(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        Name = GetType().Name;
        Message = message;
        StatusCode = code;
    }
    public string Name { get; set; }
    public string Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public static implicit operator Bunkum.HttpServer.Responses.Response(ApiError error) 
        => new(error, ContentType.Json, error.StatusCode);
}
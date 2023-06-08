using System.Net;
using System.Text.Json;
using Commerce.Api.Utils;
using Commerce.Security.Helpers.Exceptions;
using FluentValidation;
using SharpCompress.Common;

namespace Commerce.Api.Middleware;

public class ExceptionMiddleware
{
    public readonly RequestDelegate Next;
    public readonly ILogger<ExceptionMiddleware> Logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        Next = next;
        Logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await Next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception error)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        var responseModel = ApiResponse<string>.Fail(error.Message);

        switch (error)
        {
            case ValidationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                Logger.LogWarning("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case InvalidPasswordException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                Logger.LogWarning("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                Logger.LogWarning("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                Logger.LogWarning("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                Logger.LogWarning("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case ArgumentNullException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Logger.LogCritical("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case InvalidTokenException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Logger.LogCritical("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            case CryptographicException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Logger.LogCritical("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Logger.LogCritical("{Type}: {ErrorMessage}", error.GetType(), error.Message);
                break;
        }
        
        var result = JsonSerializer.Serialize(responseModel);
        await context.Response.WriteAsync(result);
    }
}
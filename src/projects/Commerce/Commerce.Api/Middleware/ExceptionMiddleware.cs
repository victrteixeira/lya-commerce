using System.Net;
using System.Text.Json;
using Commerce.Api.Utils;
using Commerce.Security.Utils;
using FluentValidation;

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
                break;
            case InvalidPasswordException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            case InvalidTokenException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
        
        Logger.LogError("{Type}: {ErrorMessage}\\n{ErrorStackTrace}", error.GetType(), error.Message, error.StackTrace);
        var result = JsonSerializer.Serialize(responseModel);
        await context.Response.WriteAsync(result);
    }
}
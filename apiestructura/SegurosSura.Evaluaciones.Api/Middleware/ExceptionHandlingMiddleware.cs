using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;

namespace SegurosSura.Evaluaciones.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = "Validation failed",
                        details = validationException.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToArray()
                    }
                };
                break;

            case SegurosSura.Evaluaciones.Domain.Exceptions.EntityNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.InnerException?.Message
                    }
                };
                break;

            case SegurosSura.Evaluaciones.Domain.Exceptions.EntityAlreadyExistsException:
                statusCode = (int)HttpStatusCode.Conflict;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.InnerException?.Message
                    }
                };
                break;

            case SegurosSura.Evaluaciones.Domain.Exceptions.ValidationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.InnerException?.Message
                    }
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                if (!_env.IsProduction())
                {
                    response = new
                    {
                        error = new
                        {
                            message = exception.Message,
                            inner = exception.InnerException?.Message,
                            details = exception.StackTrace
                        }
                    };
                }
                else
                {
                    response = new
                    {
                        error = new
                        {
                            message = "An error occurred while processing your request",
                            details = "Internal server error"
                        }
                    };
                }
                break;
        }

        context.Response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

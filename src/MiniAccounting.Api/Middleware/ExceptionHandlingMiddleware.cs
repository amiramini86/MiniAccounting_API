
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MiniAccounting.Application.Common.Responses;
using MiniAccounting.Application.Common.Exceptions;
using MiniAccounting.Domain.Exceptions;

namespace MiniAccounting.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                "Validation failed for {Path}: {Message}",
                context.Request.Path,
                ex.Message);

            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation(
                "Resource not found for {Path}: {Message}",
                context.Request.Path,
                ex.Message);

            await HandleKnownExceptionAsync(
                context,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (ConflictException ex)
        {
            _logger.LogInformation(
                "Conflict for {Path}: {Message}",
                context.Request.Path,
                ex.Message);

            await HandleKnownExceptionAsync(
                context,
                HttpStatusCode.Conflict,
                ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(
                "Business rule failed for {Path}: {Message}",
                context.Request.Path,
                ex.Message);

            await HandleKnownExceptionAsync(
                context,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.ErrorMessage).ToArray());

        var response = ApiResponse<object>.ErrorResponse(
            new ApiError("Validation failed.", errors));

        await WriteResponseAsync(
            context,
            HttpStatusCode.BadRequest,
            response);
    }

    private static async Task HandleKnownExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        var response = ApiResponse<object>.ErrorResponse(
            new ApiError(
                message,
                new Dictionary<string, string[]>()));

        await WriteResponseAsync(context, statusCode, response);
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        var response = ApiResponse<object>.ErrorResponse(
            new ApiError(
                "An unexpected error occurred.",
                new Dictionary<string, string[]>()));

        await WriteResponseAsync(
            context,
            HttpStatusCode.InternalServerError,
            response);
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        object response)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response),
            context.RequestAborted);
    }
}
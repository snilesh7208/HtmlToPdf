using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using HtmlToPdf.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HtmlToPdf.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation Failed";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = "One or more validation errors occurred.";
                    // Ideally, map validationEx.Errors to problemDetails.Extensions
                    break;
                case DataNotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Resource Not Found";
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Detail = notFoundEx.Message;
                    break;
                case DomainException domainEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Domain Rule Violation";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = domainEx.Message;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Detail = "An unexpected error occurred.";
                    break;
            }

            var result = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(result);
        }
    }
}

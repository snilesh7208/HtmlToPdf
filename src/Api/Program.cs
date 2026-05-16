using FluentValidation;
using HtmlToPdf.Api.Middleware;
using HtmlToPdf.Application.Commands;
using HtmlToPdf.Application.Interfaces;
using HtmlToPdf.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// 2. Add Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HTML to PDF API", Version = "v1" });
});

// 3. Application Layer setup (MediatR & FluentValidation)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GeneratePdfCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<HtmlToPdf.Application.Validators.GeneratePdfCommandValidator>();

// 4. Infrastructure Layer setup
builder.Services.AddSingleton<IPdfGenerator, PuppeteerPdfGenerator>();

// Setup Polly Resilience Policies for HttpClient
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.AddHttpClient<IExternalDataClient, ExternalDataClient>()
    .AddPolicyHandler(retryPolicy);

var app = builder.Build();

// 5. Middleware Pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

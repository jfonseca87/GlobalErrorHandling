using GlobalErrorHandling.API.Middlewares;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<GlobalErrorHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/throwexception", () =>
{
     throw new Exception("This is an example of an error"); 
});

app.MapGet("/unauthorized", (HttpContext context) =>
{
    var problemDetails = new ProblemDetails
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Unauthorized",
        Detail = "You are not authorized to access this resource",
        Instance = context.Request.Path
    };
    problemDetails.Extensions["traceId"] = context.TraceIdentifier;

    return Results.Problem(problemDetails);
});

app.MapPost("/badrequest", (HttpContext context, [FromBody] Person person) =>
{
    if (person == null ||
        string.IsNullOrEmpty(person?.Name) ||
        string.IsNullOrEmpty(person?.Email) || 
        (person?.Age <= 0 || person?.Age >= 110))
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = "The request has invalid fields",
            Instance = context.Request.Path
        };
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["body"] = JsonSerializer.Serialize(person);
        return Results.Problem(problemDetails);
    }

    return Results.Ok(person);
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

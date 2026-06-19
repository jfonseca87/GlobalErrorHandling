using Microsoft.AspNetCore.Mvc;

namespace GlobalErrorHandling.API.Middlewares;

public class GlobalErrorHandler(RequestDelegate next, ILogger<GlobalErrorHandler> logger)
{
    public async Task Invoke(HttpContext context, IProblemDetailsService problemDetailsService)
    {
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			var (status, title, details) = MapException(ex);
			logger.LogError(ex, "An error occurred: {Title} - {Details}", title, details);

			if (!context.Response.HasStarted)
			{
				context.Response.Clear();
				context.Response.StatusCode = status;
				context.Response.ContentType = "application/problem+json";
			}

            var problemDetails = new ProblemDetails
			{
				Status = status,
				Title = title,
				Detail = details,
				Instance = context.Request.Path
            };
			problemDetails.Extensions["traceId"] = context.TraceIdentifier;

			await problemDetailsService.WriteAsync(new ProblemDetailsContext
			{
				HttpContext = context,
				ProblemDetails = problemDetails,
				Exception = ex
            });
        }
    }

    // You can be able to add more exception types and map them accordingly
    private static (int status, string title, string details) MapException(Exception ex) => ex switch
	{
        UnauthorizedAccessException =>  (StatusCodes.Status401Unauthorized, "Unauthorized access", ex.Message),
		_ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", ex.Message)
    };
}

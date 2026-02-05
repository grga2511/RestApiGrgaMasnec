using System.Text.Json;

namespace TehnologijaWebaGrgaMasnec.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx) 
    {
        try 
        {
            await _next(ctx);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Unhandled exception");

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";

            var payload = new //javlja kakav oblik greske se dogodio
            {
                error = "Server error",
                message = ex.Message,
                path = ctx.Request.Path.Value,
                traceId = ctx.TraceIdentifier
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}

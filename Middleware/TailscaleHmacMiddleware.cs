using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using TailscaleLogIngestor.DTOs;

namespace TailscaleLogIngestor;

public class TailscaleHmacMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<TailscaleHmacMiddleware> _logger;
    public TailscaleHmacMiddleware(RequestDelegate next, ILogger<TailscaleHmacMiddleware> logger) 
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context) 
    {
        // get tailscale header
        _logger.LogInformation("Got request");

        context.Request.EnableBuffering();
        if (!context.Request.Headers.TryGetValue("Tailscale-Webhook-Signature", out StringValues extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Forbidden");
            return;
        };

        // grab body
        using StreamReader reader = new(context.Request.Body);
        string? body = await reader.ReadToEndAsync();

        // Get the shared secret
        IConfiguration appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
        string secret = appSettings.GetValue<string>("TailscaleWebhookSecret")!;
        _logger.LogInformation("Got secret with length {length}", secret.Length);

        // split up tailscale header
        var values = extractedApiKey.ToString().Split(',');

        // process timestamp
        long timestamp = long.Parse(values[0].Split('=')[1]);
        _logger.LogInformation("Got timestamp {timestamp}", timestamp);

        DateTimeOffset eventOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        _logger.LogInformation("Got event time: {eventtime}", eventOffset.DateTime);

        // if request is older than 15 mins, reject
        if (DateTime.UtcNow.AddMinutes(-15) > eventOffset.DateTime)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Request Expired");
            return;
        }

        // get signature from request
        string signature = values[1].Split('=')[1];
        _logger.LogInformation("Got signature {signature}", signature);
        byte[] sigbytes = Convert.FromHexString(signature);

        // generate our signature
        string stringToCheck = $"{timestamp}.{body}";
        _logger.LogInformation("Getting HMAC for string: {stringToCheck}", stringToCheck);
        using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(secret));
        byte[] hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(stringToCheck));

        string calculatedKey = Convert.ToHexString(hash);
        _logger.LogInformation("Signature check: Got {signature} and expected {calculcatedKey}", signature, calculatedKey);
        
        // If hashes doent match, return 401
        if (!CryptographicOperations.FixedTimeEquals(hash, sigbytes)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // hashes match, carry on
        _logger.LogInformation("Signature checked successfully");        
        context.Request.Body.Position = 0;
        await _next(context);
    }
}
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TailscaleLogIngestor.DTOs;

namespace TailscaleLogIngestor;

public class LogAnalyticsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LogAnalyticsClient> _logger;
    private readonly string _customerId;
    private readonly string _sharedKey;
    private readonly string _resource;
    private readonly string _tableName;

    public LogAnalyticsClient(HttpClient httpClient, IConfiguration configuration, ILogger<LogAnalyticsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _customerId = configuration.GetValue<string>("LogAnalyticsClient:CustomerId")!;
        _tableName = configuration.GetValue<string>("LogAnalyticsClient:TableName")!;
        _sharedKey = configuration.GetValue<string>("LogAnalyticsClient:SharedKey")!;
        _resource = "/api/logs";
        string apiHost = configuration.GetValue<string>("LogAnalyticsClient:ApiHost")!;
        string? url = $"https://{_customerId}.{apiHost}{_resource}?api-version=2016-04-01";
        _httpClient.BaseAddress = new Uri(url!);
    }

    private string BuildSignature(string message)
    {
        var encoding = new System.Text.ASCIIEncoding();
        byte[] keyByte = Convert.FromBase64String(_sharedKey);
        byte[] messageBytes = encoding.GetBytes(message);
        using var hmacsha256 = new HMACSHA256(keyByte);
        byte[] hash = hmacsha256.ComputeHash(messageBytes);
        return Convert.ToBase64String(hash);
    }

    public async Task Post(TailscaleEvent tailscaleEvent)
    {
        _logger.LogInformation("Sending event");
        string json = JsonSerializer.Serialize(tailscaleEvent);
        int messageLength = Encoding.UTF8.GetBytes(json).Length;
        string datestring = DateTime.UtcNow.ToString("r");
        string stringToHash = "POST\n" + messageLength + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
        string signature = BuildSignature(stringToHash);
        string authorization = "SharedKey " + _customerId + ":" + signature;
        
        // If charset=utf-8 is part of the content-type header, the API call may return forbidden.
        HttpContent httpContent = new StringContent(json, Encoding.UTF8);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        HttpRequestMessage request =  new(HttpMethod.Post, string.Empty);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", authorization);
        request.Headers.Add("Log-Type", _tableName);
        request.Headers.Add("x-ms-date", datestring);
        request.Headers.Add("time-generated-field", "Timestamp");
        request.Content = httpContent;

        using HttpResponseMessage response = await _httpClient.SendAsync(request);
        _logger.LogInformation("Response Code: {response}", response.StatusCode);
        response.EnsureSuccessStatusCode();  
    }
}
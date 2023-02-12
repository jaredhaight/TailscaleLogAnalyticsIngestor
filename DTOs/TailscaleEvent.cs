namespace TailscaleLogIngestor.DTOs;

public record TailscaleEvent(DateTime Timestamp, int Version, string Type, string Tailnet, string Message, object Data);
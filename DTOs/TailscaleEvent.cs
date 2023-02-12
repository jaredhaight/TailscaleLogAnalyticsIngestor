namespace TailscaleLogIngestor.DTOs;

// Detailed here: https://tailscale.com/kb/1213/webhooks/#events-payload
public record TailscaleEvent(DateTime Timestamp, int Version, string Type, string Tailnet, string Message, object Data);
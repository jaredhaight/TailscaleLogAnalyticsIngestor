# Tailscale LogAnalytics Ingestor
This is a small service that's forwards Tailscale audit events to Log Analytics. 

# Configuration
You can set the following configuration values either in appsettings.json or via environment variables.

| Setting | Default | JSON Path | Environment variable Name |
|---------|---------|-----------|---------------------------|
| Log Analytics Customer Id | None | LogAnalyticsClient.ClientId | LogAnalyticsClient__ClientId |
| Log Analytics Workspace Key | None | LogAnalyticsClient.SharedKey | LogAnalyticsClient__SharedKey |
| Log Analytics Table Name | TailscaleEvents | LogAnalyticsClient.Tablename | LogAnalyticsClient__Tablename |
| Log Analytics API Host | ods.opinsights.azure.com | LogAnalyticsClient.ApiHost | LogAnalyticsClient__ApiHost |
| Tailscale Webhook Key | None | TailscaleWebhookSecret | TailscaleWebhookSecret| 

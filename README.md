# Tailscale LogAnalytics Ingestor
This is a small service that's forwards Tailscale audit events to Log Analytics. 


# Configuration
You can set the following configuration values either in appsettings.json or via environment variables.

| Setting | Default | JSON Path | Environment variable Name |
|---------|---------|-----------|---------------------------|
| Log Analytics Workspace Id | None | LogAnalyticsClient.WorkspaceId | LogAnalyticsClient__WorkspaceId |
| Log Analytics Workspace Key | None | LogAnalyticsClient.WorkspaceKey | LogAnalyticsClient__WorkspaceKey |
| Log Analytics Table Name | TailscaleEvents | LogAnalyticsClient.Tablename | LogAnalyticsClient__Tablename |
| Log Analytics API Host | ods.opinsights.azure.com | LogAnalyticsClient.ApiHost | LogAnalyticsClient__ApiHost |
| Tailscale Webhook Key | None | TailscaleWebhookSecret | TailscaleWebhookSecret| 

## Log Analytics Workspace Id
This can be found on the Overview section of the Log Analytics workspace.

## Log Analytics Workspace Key
This is either the Primary or Secondary Key found under "Agents" > "Log Analytics agent instructions" in the Log Analytics workspace

## Log Analytics Table Name
The name of the table that tailscale events will be logged under. Default: TailscaleEvents

## Log Analytics API Host
The root hostname for log analytics. For the public azure cloud this is ods.opinsights.azure.com. Default: ods.opinsights.azure.com

## Tailscale Webhook Key
This is given to you when creating the webhook in tailscale
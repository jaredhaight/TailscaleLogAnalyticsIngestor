# Tailscale LogAnalytics Ingestor
This is a small service that's forwards Tailscale audit events to Log Analytics.

# Running
You can either build the docker image yourself by cloning the repo or you can pull the latest image from github using [these instructions](https://github.com/jaredhaight/TailscaleLogAnalyticsIngestor/pkgs/container/tailscaleloganalyticsingestor)

By default the docker container listens on 8080. You can change that by adjusting the ASPNET_ENDPOINTS environment variable or "Urls" key in appsettings.json. Details for doing that are [available here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-7.0).

Events will be in Log Analytics under the table name "TailscaleEvents_CL". You can change this by adjusting the log analytics table name setting detailed below. Log analytics will automatically add "_CL" to the end of this string.

# Configuration
You can set the following configuration values either in appsettings.json or via environment variables. Note that environment variables take precedence over the JSON configuration.

It is recommended that you store these values in a secure manner, especially the Workspace and Webhook keys.

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

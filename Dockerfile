# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# copy everything else and build app
COPY ./ ./
RUN dotnet restore && dotnet publish -c Release -o /app/out

# final stage/image
FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0-jammy-chiseled
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "TailscaleLogIngestor.dll"]
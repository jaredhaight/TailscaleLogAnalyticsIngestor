using Microsoft.AspNetCore.Mvc;
using TailscaleLogIngestor;
using TailscaleLogIngestor.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<LogAnalyticsClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<TailscaleHmacMiddleware>();

app.MapPost("/", async ([FromBody]List<TailscaleEvent> tailscaleEvents, LogAnalyticsClient client) => {
    foreach (TailscaleEvent tailscaleEvent in tailscaleEvents)
    {
        await client.Post(tailscaleEvent);
    }
});

app.Run();
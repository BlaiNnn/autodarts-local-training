using System.Windows;
using System.Threading;
using AutodartsApiSimulator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BoardSimulator>();
builder.Services.AddHostedService<ConsoleCommandService>();

var urls = builder.Configuration["ASPNETCORE_URLS"] ?? "http://0.0.0.0:3180";
builder.WebHost.UseUrls(urls);

var app = builder.Build();

app.MapGet("/api/state", (BoardSimulator sim) => Results.Json(sim.GetState()));

app.MapPost("/api/throw", (ThrowRequest req, BoardSimulator sim) =>
    Results.Json(sim.AddThrow(req.Number, req.Multiplier)));

app.MapPost("/api/random", (BoardSimulator sim) => Results.Json(sim.AddRandomThrow()));

app.MapPost("/api/reset", (BoardSimulator sim) => Results.Json(sim.Reset()));

Console.WriteLine($"Autodarts API simulator listening on {urls}");
Console.WriteLine("POST /api/throw {number, multiplier} to throw a specific segment.");
Console.WriteLine("POST /api/random to throw a random segment.");
Console.WriteLine("POST /api/reset to clear the current turn.");

await app.StartAsync();

var simulator = app.Services.GetRequiredService<BoardSimulator>();

var uiThread = new Thread(() =>
{
    var wpfApp = new Application();
    wpfApp.Run(new SimulatorWindow(simulator));
});
uiThread.SetApartmentState(ApartmentState.STA);
uiThread.Start();
uiThread.Join();

await app.StopAsync();

record ThrowRequest(int Number, int Multiplier);

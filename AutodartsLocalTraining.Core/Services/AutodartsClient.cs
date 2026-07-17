using System.Net.Http;
using System.Net.Http.Json;
using AutodartsLocalTraining.Models;

namespace AutodartsLocalTraining.Services;

public class AutodartsClient : IDisposable
{
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(2) };

    public string Ip { get; }
    public string Port { get; }

    public AutodartsClient(string ip, string port)
    {
        Ip = ip;
        Port = port;
    }

    private string StateUrl => $"http://{Ip}:{Port}/api/state";

    public async Task<AutodartsState?> GetStateAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<AutodartsState>(StateUrl, ct);

    public void Dispose() => _http.Dispose();
}

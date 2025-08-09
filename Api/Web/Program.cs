using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var proxyHost = Environment.GetEnvironmentVariable("TRUSTED_PROXY_HOST") ?? "gitnar-nginx";

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

    try
    {
        foreach (var ip in Dns.GetHostAddresses(proxyHost))
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                options.KnownProxies.Add(ip);
                options.KnownProxies.Add(ip.MapToIPv6());
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }

    options.ForwardLimit = 1; // Allow only 1 hop
    options.RequireHeaderSymmetry = true; // More friendly in container-based environment.
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/health", () => "OK");

app.Run();

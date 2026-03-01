using System.Net.WebSockets;
using System.Text;
using CryptoProj.API;
using CryptoProj.API.Endpoints;
using CryptoProj.API.Middlewares;
using CryptoProj.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

builder.Services.AddWebSockets(opt => opt.KeepAliveInterval = TimeSpan.FromSeconds(120));

builder.Services.AddTransient<GlobalExceptionHandler>();

builder.Services.AddMemoryCache();
//builder.Services.AddHostedService<TestHostedService>();
//builder.Services.AddHostedService<CryptoAnalysisHostedService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization(opt => opt
    .AddPolicy("Admin", 
        policy => policy
            .RequireClaim("role", "Admin")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthentication(); // хто ти
app.UseAuthorization();  // що ти можеш

app.MapControllers();

app.Use(async (context, next) =>
{
    Console.WriteLine(context.Request.Path);
    await next(context);
    Console.WriteLine(context.Response.StatusCode);
});

app.UseWebSockets();

app.Map("/ws", async (HttpContext context) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        return Results.Empty;

    var socket = await context.WebSockets.AcceptWebSocketAsync();
    
    while (true)
    {
        if (socket.State != WebSocketState.Open)
        {
            break;
        }

        byte[] input = new byte[1024];
        var result = await socket.ReceiveAsync(input, CancellationToken.None);
        var inputBytes = new byte[result.Count];
        Array.Copy(input, inputBytes, result.Count);
        var message = Encoding.UTF8.GetString(inputBytes);

        message = string.Join("", message.Reverse());
        
        var output = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(output, WebSocketMessageType.Text, true, CancellationToken.None);
    }
    
    return Results.Empty;
});

app.MapNewsEndpoints();

app.Run();






/*
HTTP
client -> open connection -> request -> server
server -> response -> client -> close connection

web sockets
client -> open connection -> request
response
response
response
response
response
response
response
response
response
response
response
response
 -> close connection
*/
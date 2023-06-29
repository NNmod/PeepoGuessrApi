using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.HostedServices;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Implementations.Account.Db;
using PeepoGuessrApi.Services.Implementations.Game;
using PeepoGuessrApi.Services.Implementations.Lobby;
using PeepoGuessrApi.Services.Implementations.Lobby.Db;
using PeepoGuessrApi.Services.Implementations.Maintenance.Db;
using PeepoGuessrApi.Services.Implementations.Twitch;
using PeepoGuessrApi.Services.Interfaces.Account.Db;
using PeepoGuessrApi.Services.Interfaces.Game;
using PeepoGuessrApi.Services.Interfaces.Lobby;
using PeepoGuessrApi.Services.Interfaces.Lobby.Db;
using PeepoGuessrApi.Services.Interfaces.Maintenance.Db;
using PeepoGuessrApi.Services.Interfaces.Twitch;
using IMapService = PeepoGuessrApi.Services.Interfaces.Account.Db.IMapService;
using IUserService = PeepoGuessrApi.Services.Interfaces.Account.Db.IUserService;
using MapService = PeepoGuessrApi.Services.Implementations.Account.Db.MapService;
using UserService = PeepoGuessrApi.Services.Implementations.Account.Db.UserService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Services

#region AccountDb

builder.Services.AddScoped<IDivisionService, DivisionService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameStatusService, GameStatusService>();
builder.Services.AddScoped<IGameTypeService, GameTypeService>();
builder.Services.AddScoped<IMapService, MapService>();
builder.Services.AddScoped<IRoundService, RoundService>();
builder.Services.AddScoped<IRoundSummaryService, RoundSummaryService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<IUserService, UserService>();

#endregion

#region Game

#region Db

builder.Services.AddScoped<PeepoGuessrApi.Services.Interfaces.Game.Db.IGameService, PeepoGuessrApi.Services.Implementations.Game.Db.GameService>();
builder.Services.AddScoped<PeepoGuessrApi.Services.Interfaces.Game.Db.IGameTypeService, PeepoGuessrApi.Services.Implementations.Game.Db.GameTypeService>();
builder.Services.AddScoped<PeepoGuessrApi.Services.Interfaces.Game.Db.IUserService, PeepoGuessrApi.Services.Implementations.Game.Db.UserService>();

#endregion

builder.Services.AddScoped<IFinishGameService, FinishGameService>();
builder.Services.AddScoped<IFinishRoundService, FinishRoundService>();
builder.Services.AddHttpClient<PeepoGuessrApi.Services.Interfaces.Game.IMapService, PeepoGuessrApi.Services.Implementations.Game.MapService>();
builder.Services.AddScoped<IStartRoundService, StartRoundService>();

#endregion

#region LobbyDb

#region Db

builder.Services.AddScoped<ILobbyTypeService, LobbyTypeService>();
builder.Services.AddScoped<PeepoGuessrApi.Services.Interfaces.Lobby.Db.IUserService, PeepoGuessrApi.Services.Implementations.Lobby.Db.UserService>();

#endregion

builder.Services.AddScoped<IStartGameService, StartGameService>();

#endregion

#region MaintenanceDb

builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

#endregion

#region Twitch

builder.Services.AddHttpClient<IGetUsersService, GetUsersService>();
builder.Services.AddHttpClient<IOAuth2Service, OAuth2Service>();

#endregion

#endregion

builder.Services.AddDbContextFactory<MaintenanceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MaintenanceDb")));
builder.Services.AddDbContextFactory<AccountDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountDb")));
builder.Services.AddDbContextFactory<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GameDb")));
builder.Services.AddDbContextFactory<LobbyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LobbyDb")));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddSignalR();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/var/www/ppg/persistKeysStorage"))
    .SetApplicationName("PeepoGuessrApi");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = ".PeepoGuessrApi.Authentication.Cookie"; // Provide a unique name for your authentication cookie
        options.ExpireTimeSpan = TimeSpan.FromDays(21); // Adjust the expiration time according to your needs
        options.SlidingExpiration = true; // Extend the expiration time with each request
        options.Cookie.HttpOnly = true; // Ensure that the cookie is only accessible through HTTP
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use "SameAsRequest" if your app uses HTTPS
        options.Cookie.IsEssential = true; // Allow the cookie to be used for authentication even if the user has disabled cookies
    });

builder.Services.AddHostedService<LobbyHostedService>();
builder.Services.AddHostedService<GameHostedService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<LobbyHub>("api/signalr/lobby/{lobby}", options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
});
app.MapHub<GameHub>("api/signalr/game/{game}", options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
});

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

app.MapFallbackToFile("index.html");

app.Run();
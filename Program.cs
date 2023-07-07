using System.Net;
using ValRestServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    WebSocket web = new WebSocket();
    await web.StartServer();

    
   
    RiotClientHelper riotClient = RiotClientHelper.Instance;
    await riotClient.GetAccessTokenAsync();


    // Disable SSL certificate validation
    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

    PartyChangeWatcher watcher = new PartyChangeWatcher(web);
    watcher.StartWatching();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

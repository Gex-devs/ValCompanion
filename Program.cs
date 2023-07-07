using System.Net;
using ValRestServer;
using ValRestServer.watchers;

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


    StartWatchers(web);
    
   
    RiotClientHelper riotClient = RiotClientHelper.Instance;
    await riotClient.GetAccessTokenAsync();
   

    // Disable SSL certificate validation
    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

   
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

void StartWatchers(WebSocket web)
{
    AgentSelWatcher agentSelWatcher = new AgentSelWatcher(web);
    PartyChangeWatcher watcher = new PartyChangeWatcher(web,agentSelWatcher);
    watcher.StartWatching();
    agentSelWatcher.StartWatching();
}
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ValRestServer;
using ValRestServer.watchers;

namespace ValRestServer
{
    public class RestServer
    {
        [STAThread]
        public static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();

        }

        public static async Task RunAsync()
        {
            var builder = WebApplication.CreateBuilder();

            
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

                
            }

            WebSocket web = new WebSocket();
            await web.StartServer();


            StartWatchers(web);


            RiotClientHelper riotClient = RiotClientHelper.Instance;
            await riotClient.GetAccessTokenAsync();


            // Disable SSL certificate validation
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            
            app.Run();
        }

        public static void StartWatchers(WebSocket web)
        {
            AgentSelWatcher agentSelWatcher = new AgentSelWatcher(web);
            PartyChangeWatcher watcher = new PartyChangeWatcher(web, agentSelWatcher);
            PartyChatWatcher partyChatWatcher = new PartyChatWatcher(web);

            watcher.StartWatching();
            agentSelWatcher.StartWatching();
            partyChatWatcher.StartWatching();
        }

    }
}

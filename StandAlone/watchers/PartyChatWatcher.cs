using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValRestServer.Controllers;

namespace ValRestServer.watchers
{
    public class PartyChatWatcher
    {

        private readonly HttpClient httpClient;
        private string lastData;
        private string _otherLastData;
        private WebSocket _web;
        private string previousState;
        public event EventHandler DataChanged;

        public PartyChatWatcher(WebSocket web)
        {
            httpClient = new HttpClient();
            _web = web;
            _otherLastData = string.Empty;
            lastData = string.Empty;
        }

        public void StartWatching()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    var newData = FetchData($"http://localhost:7979/api/GetPartyChat?cid={GetChatID()}");

                    if (newData != lastData)
                    {
                        // Data has changed, trigger the event
                        DataChanged?.Invoke(this, EventArgs.Empty);
                        lastData = newData;
                        HandleDataChanged(newData);
                    }

                    Thread.Sleep(500); // Wait for 1 second before the next request
                }
            });

            thread.Start();
        }

        private string FetchData(string endpointUrl)
        {
            try
            {
                var response = httpClient.GetStringAsync(endpointUrl).Result;
                return response;
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    // Handle each individual exception
                    Console.WriteLine("Exception: " + innerException.Message);
                }
            }

            return null;

        }

        private string GetChatID()
        {
            try
            {
                var response = httpClient.GetStringAsync("http://localhost:7979/api/PartyChatID").Result;
                return response;
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    // Handle each individual exception
                    Console.WriteLine("Party Chat Exception: " + innerException.Message);
                }
            }

            return null;
        }

        private void HandleDataChanged(string newData)
        {

            if (newData != null) 
            {

                Console.WriteLine("New Chat Text");
                var jsonObject = JObject.Parse(newData);

                // Get the last message object from the "messages" array
                var messagesArray = jsonObject["messages"] as JArray;
                var lastMessage = messagesArray.LastOrDefault();

                if (lastMessage != null)
                {
                    // Extract the "game_name" and "body" values
                    string gameName = (string)lastMessage["game_name"];
                    string body = (string)lastMessage["body"];

                    var data = new
                    {
                        type = "chat",
                        message = body,
                        name = gameName
                    };

                    string jsonString = JsonConvert.SerializeObject(data);

                    _web.BroadcastMessage(jsonString);
                }
            }
            
        }

    }
}

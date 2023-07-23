namespace ValRestServer.watchers
{
    public class AgentSelWatcher
    {
        private readonly HttpClient httpClient;
        private string lastData;
        private WebSocket _web;
        private string previousState;
        public event EventHandler DataChanged;
        private bool _Watch = false;

        public AgentSelWatcher(WebSocket web)
        {
            httpClient = new HttpClient();
            _web = web;
            lastData = string.Empty;
        }

        public void StartWatching()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (_Watch)
                    {
                        var newData = FetchData("http://localhost:7979/api/preGame");

                        if (newData != lastData)
                        {
                            // Data has changed, trigger the event
                            DataChanged?.Invoke(this, EventArgs.Empty);
                            lastData = newData;
                            HandleDataChanged(newData);
                        }

                        Thread.Sleep(500); // Wait for 1 second before the next request
                    }
                    
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
                    //Console.WriteLine("Exception: " + innerException.Message);
                }
            }catch(SystemException ex)
            {
                // Ingore this
            }

            return "null";

        }

        private void HandleDataChanged(string newData)
        {

            Console.WriteLine("Agent Select Changed");
            _web.BroadcastMessage(newData);
            // Perform any desired actions here
        }

        public void SetWatch(bool set)
        {
            Console.WriteLine($"Watch {set}");
            _Watch = set;
        }

    }
}

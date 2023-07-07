using System;
using System.Net.Http;
using System.Threading;
using ValRestServer;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

public class PartyChangeWatcher
{
    private readonly HttpClient httpClient;
    private string lastData;
    private string _otherLastData;
    private WebSocket _web;
    private string previousState;
    public event EventHandler DataChanged;

    public PartyChangeWatcher(WebSocket web)
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
                var newData = FetchData("http://localhost:7979/api/getParty");
                var otherData = FetchData("http://localhost:7979/api/current_state");

                if (otherData != _otherLastData)
                {
                    _otherLastData = otherData;
                    HandleCurrentStateChange(otherData);
                }

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

        return "null";
       
    }

    private void HandleDataChanged(string newData)
    {
        
        Console.WriteLine("Party data has changed!");
        _web.BroadcastMessage(newData);

        
        // Perform any desired actions here
    }

    private void HandleCurrentStateChange(string newData)
    {
        Console.WriteLine("Current State changed");
        _web.BroadcastMessage(newData);
    }
}

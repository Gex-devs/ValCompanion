using System.Net.Http;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace ValRestServer
{
    public class LocalApi
    {
 
        private static async Task<string> GetPartyChatid()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpClient _httpClient = new HttpClient(httpClientHandler);

            var url = $"https://127.0.0.1:{RiotClientHelper.Instance.GetLockFilePort()}/chat/v6/conversations/ares-parties";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {RiotClientHelper.Instance.GetBase64Chat()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(jsonResponse);

                // Get the cid value
                string cid = (string)jsonObject["conversations"][0]["cid"];
                return (cid);
            }

            return "Failed to retrieve party details";
        }

        public static async Task<string> GetPartyChat()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpClient _httpClient = new HttpClient(httpClientHandler);

  

            var url = $"https://127.0.0.1:{RiotClientHelper.Instance.GetLockFilePort()}/chat/v6/messages?cid={await GetPartyChatid()}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {RiotClientHelper.Instance.GetBase64Chat()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return jsonResponse.ToString();
            }

            return "Failed to retrieve party details";
        }


    }
}

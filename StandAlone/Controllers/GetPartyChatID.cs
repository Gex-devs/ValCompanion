using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ValRestServer.Controllers
{
    [Route("api/PartyChatID")]
    [ApiController]
    public class GetPartyChatID : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public GetPartyChatID()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<IActionResult> GetParty()
        {
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
                return Ok(cid);
            }

            return StatusCode((int)response.StatusCode, "Failed to retrieve party details");


        }
    }
}

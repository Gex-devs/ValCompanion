using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ValRestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QeueMode : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public QeueMode()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<IActionResult> GetParty()
        {
            var GetPartyID = await RiotClientHelper.Instance.GetPartyId(1);
            var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/parties/{GetPartyID}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jsondata = JObject.Parse(jsonResponse);
                string queueID = jsondata["MatchmakingData"]["QueueID"].ToString();
                return Ok(queueID);
            }

            return StatusCode((int)response.StatusCode, "Failed to retrieve party details");


        }
    }
}

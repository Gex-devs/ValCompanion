using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/pregame/gamemode")]
    public class PregameGamemodeController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PregameGamemodeController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetPregameGamemode()
        {
            var PreMatchID = await RiotClientHelper.Instance.GetPrematchId();
            var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/matches/{PreMatchID}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var j = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var queueID = j["QueueID"].ToString();

                return queueID;
            }

            return BadRequest("Failed to retrieve pregame gamemode");
        }
    }
}

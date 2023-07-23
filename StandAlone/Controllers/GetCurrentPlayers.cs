using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/coregame/players")]
    public class CurrentPlayersController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public CurrentPlayersController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetCurrentPlayers()
        {
            var GetCurrentGameID = await RiotClientHelper.Instance.GetCurrentGameId();
            var url = $"https://glz-eu-1.eu.a.pvp.net/core-game/v1/matches/{GetCurrentGameID}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var players = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(jsonResponse);
                return players;
            }

            return BadRequest("Failed to retrieve current players");
        }
    }
}

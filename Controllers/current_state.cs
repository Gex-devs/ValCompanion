using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ValRestServer.Controllers
{
    [Route("api/current_state")]
    [ApiController]
    public class CurrentStateController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public CurrentStateController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetGameStatus()
        {
            var playerId = RiotClientHelper.Instance.GetPlayerID();
            var currentGameUrl = $"https://glz-eu-1.eu.a.pvp.net/core-game/v1/players/{playerId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var currentGameResponse = await _httpClient.GetAsync(currentGameUrl);

            if (currentGameResponse.IsSuccessStatusCode)
            {
                return Ok("In_Game");
            }

            var preGameUrl = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/players/{RiotClientHelper.Instance.GetPlayerID()}";

            var preGameResponse = await _httpClient.GetAsync(preGameUrl);

            if (preGameResponse.IsSuccessStatusCode)
            {
                return Ok("Agent_sel");
            }

            return Ok("MainMenu");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/Dodge")]
    public class DodgeGameController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public DodgeGameController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpPost]
        public async Task<ActionResult<int>> DodgeGame()
        {
            var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/matches/{RiotClientHelper.Instance.GetPreMatchId()}/quit";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.PostAsync(url, null);

            return (int)response.StatusCode;
        }
    }
}

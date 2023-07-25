using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/startQ")]
    public class StartQController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public StartQController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> StartQ()
        {
            var partyId = await RiotClientHelper.Instance.GetPartyId(1);
            var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/parties/{partyId}/matchmaking/join";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.PostAsync(url, null);
            if (response.IsSuccessStatusCode) {
                return Ok(response.StatusCode.ToString());
            }
            return BadRequest(response.StatusCode.ToString());
        }
    }
}

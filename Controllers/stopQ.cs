using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/stopQ")]
    public class StopQController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public StopQController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<int>> StopQ()
        {
            var partyId = await RiotClientHelper.Instance.GetPartyId(1);
            var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/parties/{partyId}/matchmaking/leave";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.PostAsync(url, null);

            return (int)response.StatusCode;
        }
    }
}

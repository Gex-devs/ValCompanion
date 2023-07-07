using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/party_accessibility")]
    public class PartyAccessibilityController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PartyAccessibilityController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> PartyAccessibility(string state)
        {
            var partyId = await RiotClientHelper.Instance.GetPartyId(1);
            var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/parties/{partyId}/accessibility";

            var payload = new
            {
                accessibility = state
            };

            var jsonPayload = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.PostAsync(url, jsonPayload);

            return response.StatusCode.ToString();
        }
    }
}

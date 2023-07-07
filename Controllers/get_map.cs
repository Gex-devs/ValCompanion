using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/get_map")]
    public class get_map : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public get_map()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetMap()
        {
            var preMatchId = await RiotClientHelper.Instance.GetPreMatchId();
            var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/matches/{preMatchId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var mapID = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse)["MapID"].ToString();
                return Ok(mapID);
            }

            return BadRequest("Failed to retrieve map");
        }

    }
}

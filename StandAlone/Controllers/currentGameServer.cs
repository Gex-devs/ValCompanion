using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/get_server/pre_game")]
    public class GetCurrentServerPreGameController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GetCurrentServerPreGameController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetCurrentServerPreGame()
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
                var gamePodID = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse)["GamePodID"].ToString();

                var server = string.Empty;

                switch (gamePodID)
                {
                    case "aresriot.aws-euc1-prod.eu-gp-frankfurt-1":
                        server = "Frankfurt";
                        break;
                    case "aresriot.aws-apne1-prod.eu-gp-tokyo-1":
                        server = "Tokyo";
                        break;
                    case "aresriot.aws-mes1-prod.eu-gp-bahrain-1":
                        server = "Bahrain";
                        break;
                    case "aresriot.aws-rclusterprod-mad1-1.eu-gp-madrid-1":
                        server = "Madrid";
                        break;
                    case "aresriot.aws-euw3-prod.eu-gp-paris-1":
                        server = "Paris";
                        break;
                    case "aresriot.aws-eun1-prod.eu-gp-stockholm-1":
                        server = "Stockholm";
                        break;
                    case "aresriot.mtl-riot-ist1-2.eu-gp-istanbul-1":
                        server = "Istanbul";
                        break;
                    case "aresriot.aws-euw2-prod.eu-gp-london-1":
                        server = "London";
                        break;
                    case "aresriot.aws-rclusterprod-waw1-1.eu-gp-warsaw-1":
                        server = "Warsaw";
                        break;
                }

                return server;
            }

            return BadRequest("Failed to retrieve current server");
        }
    }
}

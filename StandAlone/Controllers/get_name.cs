using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/get_name")]
    public class UsernameController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public UsernameController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetUsername(string PUID)
        {
           
            var url = "https://pd.eu.a.pvp.net/name-service/v2/players";
            var payload = new string[] { PUID };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var jsonPayload = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, jsonPayload);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var gameName = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse)[0]["GameName"].ToString();
                return Ok(gameName);
            }

            return BadRequest("Failed to retrieve username");
        }
    }
}

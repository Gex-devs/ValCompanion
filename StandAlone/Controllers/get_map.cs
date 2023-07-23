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
            var preMatchId = String.Empty;   
            try
            {
                preMatchId = await RiotClientHelper.Instance.GetPreMatchId();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
            var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/matches/{preMatchId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            HttpResponseMessage response = null;

            if (preMatchId != String.Empty)
            {
                response = await _httpClient.GetAsync(url);
            }
            

             if(response != null && response.IsSuccessStatusCode  )
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var mapID = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse)["MapID"].ToString();
                return Ok(mapID);

            }else {
                var currentMatchId = await RiotClientHelper.Instance.GetCurrentGameId();
                Console.WriteLine("current Match id " + currentMatchId);
                url = $"https://glz-eu-1.eu.a.pvp.net/core-game/v1/matches/{currentMatchId}";

                response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var mapID = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse)["MapID"].ToString();
                    return Ok(mapID);
                }

            }

            return BadRequest("Failed to retrieve map");
        }

    }
}

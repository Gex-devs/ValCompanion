using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ValRestServer.Controllers
{
    [Route("api/GetPartyChat")]
    [ApiController]
    public class GetPartyChat : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public GetPartyChat()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<IActionResult> GetParty(string cid)
        {
            var GetPartyID = await RiotClientHelper.Instance.GetPartyId(1);
            var url = $"https://127.0.0.1:{RiotClientHelper.Instance.GetLockFilePort()}/chat/v6/messages?cid={cid}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {RiotClientHelper.Instance.GetBase64Chat()}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return Ok(jsonResponse);
            }

            return StatusCode((int)response.StatusCode, "Failed to retrieve party details");


        }
    }
}

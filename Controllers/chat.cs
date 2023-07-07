using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/sendChat")]
    public class SendChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public SendChatController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

   
        [HttpGet]
        public async Task<ActionResult<int>> SendChat(string text)
        {
            var url = $"https://127.0.0.1:{RiotClientHelper.Instance.GetLockFilePort()}/chat/v6/messages/";

            var payload = new
            {
                cid = await RiotClientHelper.Instance.GetPartyId(0),
                message = text,
                type = "groupchat"
            };

            var jsonPayload = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {RiotClientHelper.Instance.GetBase64Chat()}");

            var response = await _httpClient.PostAsync(url, jsonPayload);

            return (int)response.StatusCode;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/leave_party")]
    public class LeavePartyController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LeavePartyController()
        {
            _httpClient = new HttpClient();
        }

        [HttpDelete]
        public async Task<ActionResult<int>> LeaveParty()
        {
            var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/players/{RiotClientHelper.Instance.GetPlayerID()}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.DeleteAsync(url);

            return (int)response.StatusCode;
        }
    }
}

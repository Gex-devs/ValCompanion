using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class PartyController : ControllerBase
{
    // Your existing code for getting Player_ID, Entitlment, and Authorization
    private readonly HttpClient _httpClient;
    public PartyController() {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        _httpClient = new HttpClient(httpClientHandler);
    }

    [HttpGet]
    [Route("api/getParty")]
    public async Task<IActionResult> GetParty()
    {
        var GetPartyID = await RiotClientHelper.Instance.GetPartyId(1);
        var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/parties/{GetPartyID}";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return Ok(jsonResponse);
        }

        return StatusCode((int)response.StatusCode, "Failed to retrieve party details");

     
    }


}

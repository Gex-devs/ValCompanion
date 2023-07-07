﻿using DnsClient;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValRestServer.Controllers
{
    [ApiController]
    [Route("api/pregame/lockagent")]
    public class LockAgentController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LockAgentController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpGet]
        public async Task<ActionResult<string>> LockAgent(string agent)
        {
            var PreMatchID = await RiotClientHelper.Instance.GetPrematchId();
            var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/matches/{PreMatchID}/lock/{agent}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", RiotClientHelper.Instance.GetEntitlement());
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RiotClientHelper.Instance.GetAuthorization()}");

            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Locked Agent: " + agent);
                return agent;
            }

            return BadRequest("Failed to lock agent");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ValRestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMyID : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return RiotClientHelper.Instance.GetPlayerID();
        }
    }
}

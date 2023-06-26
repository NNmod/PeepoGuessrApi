using Microsoft.AspNetCore.Mvc;

namespace PeepoGuessrApi.Controllers;

[ApiController]
[Route("api/status")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> Check()
    {
        return Task.FromResult<IActionResult>(Ok("Working..."));
    }
}
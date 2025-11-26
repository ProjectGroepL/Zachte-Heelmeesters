using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("config-test")]
    public IActionResult TestConfig([FromServices] IConfiguration config)
    {
        return Ok(new {
            smtp = config["SMTP:User"],
            secret = config["TwoFactorSecret"]
        });
    }

    [HttpGet("mail-test")]
    public async Task<IActionResult> TestMail([FromServices] IEmailSender mail)
    {
        await mail.SendAsync("test@example.com", "Test email", "Hello from ZHM!");
        return Ok("Email verstuurd!");
    }
    
}


}

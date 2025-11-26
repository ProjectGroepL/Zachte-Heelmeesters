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
    private readonly ApiContext _db;
    private readonly IEmailSender _mail;
    private readonly IConfiguration _config;

        public TestController(ApiContext db, IEmailSender mail, IConfiguration config)
        {
           _db = db;
            _mail = mail;
            _config = config;
        }

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
        await mail.SendAsync("m.voets1@student.avans.nl", "Test email", "Hello from ZHM!");
        return Ok("Email verstuurd!");
    }
    
}


}

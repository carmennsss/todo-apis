using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using todo_apis.Entities.Models;
using todo_apis.Services.Interfaces;

namespace todo_apis.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : Controller
    {
        public static ClientDto client = new();

        // HTTP POSTS -------

        [HttpPost("register")]
        public async Task<ActionResult<ClientDto>> Register(ClientDto request)
        {
            var client = await authService.RegisterAsync(request);
            if (client is null)
            {
                return BadRequest("User not found");
            }
            return Ok(client);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(ClientDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
            {
                return BadRequest("Invalid username/password");
            }
            return Ok(token);
        }

        // HTTP GETS -------

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnly()
        {
            return Ok("You are Authenticated");
        }
    }
}

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

        /// <summary>
        /// Gets the client and calls the auth service (register method)
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Ok status with the client or BadRequest
        /// </returns>
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

        /// <summary>
        /// Gets the client and calls the auth service (login method)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Ok status with the token or BadRequest
        /// </returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(ClientDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
            {
                return BadRequest("Invalid username/password");
            }
            return Ok( new { token = token });
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

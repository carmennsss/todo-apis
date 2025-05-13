using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using todo_apis.Context;
using todo_apis.Entities.Models;
using todo_apis.Models;
using todo_apis.Services.Interfaces;

namespace todo_apis.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<Client?> RegisterAsync(ClientDto request)
        {
            if (await context.clients.AnyAsync(c => c.username == request.username))
            {
                return null;
            }
            var client = new Client();
            var hashedPassword = new PasswordHasher<Client>()
                .HashPassword(client, request.password);
            client.username = request.username;
            client.passwordHashed = hashedPassword;

            context.clients.Add(client);
            await context.SaveChangesAsync();
            return client;
        }

        public async Task<string?> LoginAsync(ClientDto request)
        { 
            var client = await context.clients.FirstOrDefaultAsync(c => c.username == request.username);
            if (client == null)
            {
                return null;
            }
            if (client.username != request.username)
            {
                return null;
            }
            if (new PasswordHasher<Client>().VerifyHashedPassword(client, client.passwordHashed, request.password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return CreateToken(client);
        }

        private string CreateToken(Client client)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, client.username)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}

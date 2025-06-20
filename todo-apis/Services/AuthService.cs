﻿using System.Configuration;
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
        /**
         * Searches the username, if it already exists returns null.
         * If not, hashes the password and adds the client to the db.
         */
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

        /**
         * Searches the username, if it exists, 
         * verifies the password and creates a token
         */
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

        /**
         * Creates the token with the config settings and sets the claims.
         */
        private string CreateToken(Client client)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, client.username),
                new Claim(JwtRegisteredClaimNames.Sub, client.username),
                new Claim(ClaimTypes.NameIdentifier, client.username)
            };
            var expiresInMinutes = configuration.GetValue<int>("AppSettings:ExpiresAt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}

using LaundryCleaning.Service.Auth.CustomModels;
using LaundryCleaning.Service.Auth.Services.Interfaces;
using LaundryCleaning.Service.Common.Exceptions;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LaundryCleaning.Service.Auth.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext dbContext,
            IPasswordService passwordService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthLoginCustomModel> AuthLogin(LoginRequest input, CancellationToken cancellationToken)
        {
            var userExist = await _dbContext.Users.Where(x => x.Email.Equals(input.Email)).FirstOrDefaultAsync(cancellationToken);

            if (userExist == null)
            {
                throw new BusinessLogicException("Email not found!");
            }

            bool isValid = false;

            isValid = _passwordService.VerifyPassword(userExist.Password, input.Password);

            //if (userExist.Password == "test") isValid = true; // for testing

            var response = new AuthLoginCustomModel();
            if (isValid)
            {
                var token = await GenerateJwt(userExist, cancellationToken);

                response = new AuthLoginCustomModel()
                {
                    Success = true,
                    Token = token
                };
            }
            else
            {
                throw new BusinessLogicException("Wrong Password!");
            }

            return response;
        }

        private async Task<string> GenerateJwt(User user, CancellationToken cancellationToken)
        {
            var userWithRoles = await _dbContext.Users
                .Include(u => u.Identities)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            /*
            var json = JsonSerializer.Serialize(userWithRoles, new JsonSerializerOptions
            {
                WriteIndented = true, // biar rapi
                ReferenceHandler = ReferenceHandler.IgnoreCycles // cegah infinite loop
            });

            _logger.LogInformation("UserWithRoles: {Json}", json);

            if (userWithRoles == null)
            {
                throw new BusinessLogicException("Roles not found!");
            }
            */

            var permissions = userWithRoles.Identities
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToList();

            /*
            var json2 = JsonSerializer.Serialize(permissions, new JsonSerializerOptions
            {
                WriteIndented = true, // biar rapi
                ReferenceHandler = ReferenceHandler.IgnoreCycles // cegah infinite loop
            });

            _logger.LogInformation("permissions: {Json}", json2);
            */

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("name", user.Username),
            };
            claims.AddRange(permissions.Select(p => new Claim("permission", p)));

            var token = new JwtSecurityToken(
                        issuer: _configuration["JwtSettings:Issuer"],
                        audience: _configuration["JwtSettings:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(2),
                        signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

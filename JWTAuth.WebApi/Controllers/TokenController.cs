using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using JWTAuth.WebApi.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuth.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseContext _context;

    public TokenController(IConfiguration configuration, DatabaseContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Post(UserInfo _userData)
    {
        if (_userData is null || _userData.Email is null || _userData.Password is null)
        {
            return BadRequest();
        }

        var user = await GetUser(_userData.Email, _userData.Password);

        if (user is null)
            return BadRequest("Invalid credentials");

        // Skapa claims detaljer baserat på användarens info
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("DisplayName", user.DisplayName),
            new Claim("UserName", user.UserName),
            new Claim("Email", user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signIn);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }

    private async Task<UserInfo> GetUser(string email, string password)
    {
        return await _context.UserInfos.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }
}

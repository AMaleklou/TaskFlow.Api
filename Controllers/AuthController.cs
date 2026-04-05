using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Api.Data;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Models;
using TaskFlow.Api.Common.Exceptions;
using TaskFlow.Api.Common;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly IConfiguration _config;
        private readonly AppDbContext _appDbContext;

        public AuthController(IConfiguration config, AppDbContext appDbContext) 
        {
          _config = config;
          _appDbContext = appDbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user =await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new BadRequestException("Invalid username or password");

            var token = GenerateJwtToken(user);
            return Ok(ApiResponse.Success(new { token }, "Login successful"));
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existUser = await _appDbContext.Users.AnyAsync(u => u.Username == dto.Username);
            if (existUser) throw new BadRequestException("This Username Already Existed !!!");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)

            };

            _appDbContext.Users.Add(user);
           await _appDbContext.SaveChangesAsync();

            return Ok(ApiResponse.Success<object>(null, "User created successfully"));
        }
    }
}

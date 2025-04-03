using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NegotiationAPI.Models;

namespace NegotiationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly List<Employee> _employees = new()
        {
            new Employee { Id = 1, Username = "admin", Password = "admin123" }
        };

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] EmployeeLoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Username and password are required");

            var employee = _employees.FirstOrDefault(e =>
                e.Username == loginDto.Username &&
                e.Password == loginDto.Password);

            if (employee == null)
                return Unauthorized("Invalid username or password");

            var token = GenerateJwtToken(employee);

            return Ok(new
            {
                Token = token,
                ExpiresIn = _config.GetValue<int>("Jwt:ExpiryInMinutes") * 60
            });
        }

        private string GenerateJwtToken(Employee employee)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name, employee.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:ExpiryInMinutes")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
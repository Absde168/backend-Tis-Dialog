using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using deaplom.Model;
using deaplom.Helpers;

namespace deaplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TisDialogContext _context;
        private readonly string _secretKey = "YourVerySecureSecretKey123!@#Loshnya";

        public AuthController(TisDialogContext context)
        {
            _context = context;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Логин и пароль обязательны");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == request.Login);

            // Проверяем пароль через BCrypt — сравниваем с хешем из БД
            if (user == null || !PasswordHelper.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Неверный логин или пароль");

            var token = GenerateJwt(user);
            return Ok(new { token });
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Логин и пароль обязательны");

            var exists = await _context.Users.AnyAsync(u => u.Login == request.Login);
            if (exists)
                return Conflict("Пользователь с таким логином уже существует");

            var user = new User
            {
                Login            = request.Login,
                PasswordHash     = PasswordHelper.Hash(request.Password), // хешируем BCrypt
                FirstName        = request.FirstName,
                LastName         = request.LastName,
                Email            = request.Email,
                Phone            = request.Phone,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwt(user);
            return Ok(new { token, userId = user.Id });
        }

        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            };

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:            "TisDialog",
                audience:          "MobileApp",
                claims:            claims,
                expires:           DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Login    { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Login     { get; set; } = string.Empty;
        public string Password  { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public string Email     { get; set; } = string.Empty;
        public string Phone     { get; set; } = string.Empty;
    }
}

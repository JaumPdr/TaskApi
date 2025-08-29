using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Controllers
{
    // Define a rota base para este controlador. O "[controller]" será substituído pelo nome do controlador ("Auth").
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // O construtor injeta o DbContext, permitindo a comunicação com o banco de dados.
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Endpoint de registro. Responde a requisições POST para a URL: /api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Verifica se um usuário com o mesmo nome já existe.
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                // Se já existe, retorna um erro.
                return BadRequest("Usuário já existe.");
            }

            // Cria um novo usuário e armazena a senha como hash
            var user = new User
            {
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "User"
            };

            // Adiciona o novo usuário ao contexto de banco de dados.
            _context.Users.Add(user);

            // Salva as alterações no banco de dados de forma assíncrona.
            await _context.SaveChangesAsync();

            // Retorna uma resposta de sucesso.
            return Ok("Usuário registrado com sucesso.");
        }
        // Endpoint para o login do usuário.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Procura o usuário no banco de dados pelo nome de usuário.
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

            // Verifica se o usuário existe e se a senha está correta usando BCrypt.Verify().
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            // Se o login for válido, gera um token JWT.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Obtém a chave secreta de appsettings.json

            // Define as informações que serão incluídas no token (como o nome do usuário).
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1), // O token expira em 1 hora.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Retorna o token para o front-end.
            return Ok(new { Token = tokenString });
        }
    }
}

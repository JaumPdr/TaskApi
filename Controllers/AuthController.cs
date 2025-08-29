using Microsoft.AspNetCore.Mvc;
using TaskApi.Data;
using TaskApi.Models;
using BCrypt.Net;
using System.Linq;
using System.Threading.Tasks;

namespace TaskApi.Controllers
{
    // Define a rota base para este controlador.
    // O "[controller]" será substituído pelo nome do controlador ("Auth").
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // O construtor injeta o DbContext, permitindo a comunicação com o banco de dados.
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint de registro. Responde a requisições POST para a URL: /api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Verifica se um usuário com o mesmo nome já existe.
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                // Se já existe, retorna um erro.
                return BadRequest("Usuário já existe.");
            }

            // Gera o hash da senha usando BCrypt, que é mais seguro do que a senha em texto puro.
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            // Adiciona o novo usuário ao contexto de banco de dados.
            _context.Users.Add(user);

            // Salva as alterações no banco de dados de forma assíncrona.
            await _context.SaveChangesAsync();

            // Retorna uma resposta de sucesso.
            return Ok("Usuário registrado com sucesso.");
        }
    }
}

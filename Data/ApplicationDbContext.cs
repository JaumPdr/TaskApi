// Importa o Entity Framework Core, a principal biblioteca para acessar o banco de dados.
using Microsoft.EntityFrameworkCore;
// Importa os modelos de dados (classes como User e Task) da pasta 'Models'.
using TaskApi.Models;

namespace TaskApi.Data
{
    // A classe ApplicationDbContext herda de DbContext para funcionar como a ponte entre a aplicação e o banco de dados.
    public class ApplicationDbContext : DbContext
    {
        // Construtor que recebe as opções de configuração e as passa para a classe base (DbContext).
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Representa a tabela 'Users' no banco de dados.
        public DbSet<User> Users { get; set; }

        // Representa a tabela 'Tasks' no banco de dados.
        public DbSet<Models.Task> Tasks { get; set; }
    }
}

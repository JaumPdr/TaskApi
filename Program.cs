using Microsoft.EntityFrameworkCore;
using TaskApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços da aplicação ao contêiner de injeção de dependência.

// Adiciona o suporte para controllers na API.
builder.Services.AddControllers();

// Configura o Entity Framework Core para usar o SQL Server e a string de conexão "DefaultConnection".
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os serviços para gerar a documentação da API usando o Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o CORS (Cross-Origin Resource Sharing) para permitir requisições de outras origens.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // Permite requisições do frontend React (porta padrão do Vite).
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader() // Permite qualquer cabeçalho na requisição.
                  .AllowAnyMethod(); // Permite qualquer método HTTP (GET, POST, etc.).
        });
});

var app = builder.Build();

// Configura o pipeline de requisições HTTP.

// Ativa os middlewares do Swagger apenas no ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona todas as requisições HTTP para HTTPS.
app.UseHttpsRedirection();

// Habilita a política de CORS configurada acima.
app.UseCors();

// Habilita a autenticação e autorização na aplicação.
app.UseAuthorization();

// Mapeia os controladores da API para as rotas.
app.MapControllers();

// Inicia a aplicação.
app.Run();

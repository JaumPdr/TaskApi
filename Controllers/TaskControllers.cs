using Microsoft.AspNetCore.Mvc;
using TaskApi.Data;
using TaskApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace TaskApi.Controllers
{
    //Rota base para estre controlador
    [Route("api/[controller]")]
    [ApiController]
    //Linha abaixo protege este controlador. Apenas requisições com um jwt válido podem acessá-lo.
    [Authorize]
    public class TasksControllers : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // O construtor injeta o nosso DbContext, que é a ponte para o banco de dados.
        public TasksControllers(ApplicationDbContext context)
        {
            _context = context;
        }

        //--- Método READ: Obter todas as tarefas do usuário logado ---
        //Rota será GET /api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            //Pega o ID do usuário a partir do token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Busca todas as tarefas que pertencem a este usuário
            var tasks = await _context.Tasks
                .Where(t => t.UserId.ToString() == userId)
                .ToListAsync();

            return Ok(tasks);
        }

        //--- Método CREATE: Criar uma nova tarefa ---
        //Rota será POST /api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] Models.Task task)
        {
            //Pega o ID do usuário logado e associa a tarefa dele
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            task.UserId = int.Parse(userId); // Converte o ID de string para int.

            _context.Tasks.Add(task); // Adiciona a nova tarefa ao contexto do banco.
            await _context.SaveChangesAsync(); // Salva as mudanças de forma assíncrona.

            // Retorna um status, indicando que a tarefa foi criada. O retorno também inclui a URL para acessar a nova tarefa.
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }

        //--- Método UPDATE: Atualizar uma tarefa existente ---
        //Rota será PUT api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] Models.Task updatedTask)
        {
            // Verifica se o ID na URL corresponde ao ID no corpo da requisição.
            if (id != updatedTask.Id)
            {
                return BadRequest(); // Retorna 400 Bad Request se não corresponder.
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Busca a tarefa no banco de dados e verifica se ela pertence ao usuário logado.
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId.ToString() == userId);

            // Se a tarefa não for encontrada ou não pertencer ao usuário, retorna 404 Not Found.
            if (existingTask == null)
            {
                return NotFound();
            }

            // Atualiza as propriedades da tarefa existente com os novos dados.
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.IsCompleted = updatedTask.IsCompleted;

            _context.Tasks.Update(existingTask); // Marca a tarefa para ser atualizada.
            await _context.SaveChangesAsync(); // Salva as mudanças.

            // Retorna um status 204 NoContent, que é a resposta padrão para uma atualização bem-sucedida sem conteúdo.
            return NoContent();
        }

        //--- Método DELETE: Deletar uma tarefa ---
        //Rota será DELETE /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Busca a tarefa pelo ID e pelo ID do usuário.
            var taskToDelete = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId.ToString() == userId);

            // Se a tarefa não for encontrada ou não pertencer ao usuário, retorna 404.
            if (taskToDelete == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskToDelete);// Marca a tarefa para ser removida.
            await _context.SaveChangesAsync(); // Salva as mudanças.

            // Retorna um status 204 NoContent, indicando a remoção bem-sucedida.
            return NoContent();
        }
    
    
    
    
    
    
    
    
    
    
    
    
    }
}

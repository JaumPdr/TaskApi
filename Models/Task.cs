namespace TaskApi.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        //Chave estrangeira para o usuário
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}

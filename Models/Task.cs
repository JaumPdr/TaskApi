namespace TaskApi.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Descripition { get; set; }
        public bool IsCompleted { get; set; }

        //Chave estrangeira para o usuário
        public int UderId { get; set; }
        public User? User { get; set; }
    }
}

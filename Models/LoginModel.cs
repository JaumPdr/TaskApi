using Microsoft.Data.SqlClient;

namespace TaskApi.Models
{
    public class LoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities.Models
{
    public class ClientDto
    {
        [Key]
        public string username { get; set; }

        public string password { get; set; }

        public ClientDto(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        public ClientDto() { }
    }
}

using System.ComponentModel.DataAnnotations;
namespace todo_apis.Models
{
    public class Client
    {
        [Key]
        public string username { get; set; }

        public string passwordHashed { get; set; }
    }
}
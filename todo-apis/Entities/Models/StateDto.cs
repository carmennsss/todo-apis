using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities.Models
{
    public class StateDto
    {
        [Key]
        public string name { get; set; }
    }
}

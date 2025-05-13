using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities.Models
{
    public class SubTaskDto
    {
        [Key]
        public long subtask_id { get; set; }

        public string subtask_name { get; set; }
    }
}

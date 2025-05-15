using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities.Models
{
    public class Task_TagDto
    {
        public int task_id { get; set; }

        public int tag_id { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities.Models
{
    public class Task_SubtaskDto
    {
        public int task_id { get; set; }

        public int subtask_id { get; set; }
    }
}

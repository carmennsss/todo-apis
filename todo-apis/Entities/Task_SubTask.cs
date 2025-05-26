using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class Task_SubTask
    {
        [Key]
        public int subtask_id { get; set; }

        public string subtask_name { get; set; }
        public int task_id { get; set; }
    }
}

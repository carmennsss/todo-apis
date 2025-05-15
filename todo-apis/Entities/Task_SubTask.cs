using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class Task_SubTask
    {
        public int task_id { get; set; }

        public int subtask_id { get; set; }
    }
}

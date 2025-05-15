using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class Task_Tag
    {
        public int task_id { get; set; }

        public int tag_id { get; set; }
    }
}

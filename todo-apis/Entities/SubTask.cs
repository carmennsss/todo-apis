using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class SubTask
    {
        [Key]
        public long subtask_id { get; set; }

        public string subtask_name { get; set; }
    }
}

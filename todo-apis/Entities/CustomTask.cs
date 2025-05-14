using System.ComponentModel.DataAnnotations;

namespace todo_apis.Models
{
    public class CustomTask
    {
        [Key]
        public long task_id { get; set; }

        public string task_name { get; set; }
        public string task_desc { get; set; }
        public int list_id { get; set; }
        public string task_due_date { get; set; }
        public string state_name { get; set; }
        public string client_user { get; set; }

    }
}

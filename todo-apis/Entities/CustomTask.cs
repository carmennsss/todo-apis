using System.ComponentModel.DataAnnotations;

public class CustomTask
{
    [Key]
    public int task_id { get; set; }

    public string task_name { get; set; }
    public string task_desc { get; set; }
    public int? list_id { get; set; }
    public DateTime? task_due_date { get; set; }
    public string state_name { get; set; }
    public string client_user { get; set; }

    public CustomTask(string name, string desc, int? list_id, DateTime? due_date, string state, string user)
    {
        task_name = name;
        task_desc = desc;
        this.list_id = list_id;
        task_due_date = due_date;
        state_name = state;
        client_user = user;
    }

    public CustomTask() { }
}

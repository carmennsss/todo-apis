﻿using System.ComponentModel.DataAnnotations;

public class TaskDto
{
    [Key]
    public int task_id { get; set; }

    public string task_name { get; set; }
    public string task_desc { get; set; }

    public int? list_id { get; set; }
    public DateTime? task_due_date { get; set; }
    public string state_name { get; set; }

    public TaskDto(string task_name, string task_desc, int? list_id, DateTime? task_due_date, string state_name)
    {
        this.task_name = task_name;
        this.task_desc = task_desc;
        this.list_id = list_id;
        this.task_due_date = task_due_date;
        this.state_name = state_name;
    }

    public TaskDto() { }
}

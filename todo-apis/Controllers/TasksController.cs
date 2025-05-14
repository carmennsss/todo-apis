using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Models;

namespace todo_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoTasks : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoTasks(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomTask>> Posttasks(CustomTask tasks)
        {
            _context.tasks.Add(tasks);
            await _context.SaveChangesAsync();
            return Ok(tasks);
            //return CreatedAtAction(nameof(GetTasks), new { id = tasks.task_id }, tasks);
            //return CreatedAtAction("Gettasks", new { id = tasks.Id }, tasks);
        }

        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksFromClient(string username)
        {

            var client =  await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == client.username).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [HttpGet("{date}/{username}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksFromClient(string username, string date)
        {
            if (date == null)
            {
                return BadRequest("Date Not Found");
            }

            var client = await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == client.username && task.task_due_date == date).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [HttpPost("edit/{task_id}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksFromClient(long task_id, CustomTask edited_Task)
        {
            foreach (var task in _context.tasks.Where(task => task_id == task.task_id))
            {
                if (task == null)
                {
                    return BadRequest("Task Not Found");
                }

                if (edited_Task.task_name != "")
                {
                    task.task_name = edited_Task.task_name;
                }

                if (edited_Task.task_desc != "")
                {
                    task.task_desc = edited_Task.task_desc;
                }

                if (edited_Task.list_id != -1)
                {
                    var list = _context.categories.Where(category => category.category_id == edited_Task.list_id).FirstOrDefault();
                    if (list == null)
                    {
                        return BadRequest("Category Not Found");
                    }
                    task.list_id = edited_Task.list_id;
                }
            }
            return Ok();
        }

    }
}

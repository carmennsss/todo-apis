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
        public async Task<ActionResult<CustomTask>> PostTasks(CustomTask tasks)
        {
            _context.tasks.Add(tasks);
            await _context.SaveChangesAsync();
            return Ok(tasks);
            //return CreatedAtAction(nameof(GetTasks), new { id = tasks.task_id }, tasks);
            //return CreatedAtAction("Gettasks", new { id = tasks.Id }, tasks);
        }

        [HttpGet("status/{status}/{username}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksStatus(string username, string state_name)
        {

            var client = await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == client.username && task.state_name == state_name).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [HttpGet("category/{category}/{username}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksCategory(string username, int category_id)
        {

            var client = await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }

            var category = await _context.categories.FindAsync(category_id);
            if (category == null)
            {
                return BadRequest("Category Not Found");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == client.username && task.list_id == category.category_id).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }

            return Ok(tasks);
        }

        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksClient(string username)
        {

            var client = await _context.clients.FindAsync(username);
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
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksDate(string username, string date)
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
        public async Task<ActionResult<IEnumerable<TaskDto>>> EditTask(int task_id, CustomTask edited_Task)
        {
            var task = await _context.tasks.FindAsync(task_id);
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
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int task_id)
        {
            var task = await _context.tasks.FindAsync(task_id);
            if (task == null)
            {
                return BadRequest("Task Not Found");
            }
            return Ok(task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

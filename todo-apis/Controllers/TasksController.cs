using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Models;
using todo_apis.Services.Interfaces;

namespace todo_apis.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class Tasks : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITasksService tasksService;

        public Tasks(AppDbContext context, ITasksService tasksService)
        {
            _context = context;
            this.tasksService = tasksService;
        }



        // HTTP GETS -------

        [Authorize]
        [HttpGet("user/status/{status-name}")]
        public async Task<ActionResult<IEnumerable<CustomTask>>> GetTasksStatus(string state_name)
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username && task.state_name == state_name).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("user/category/{id-category}")]
        public async Task<ActionResult<IEnumerable<CustomTask>>> GetTasksCategory(int category_id)
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var category = await _context.categories.FindAsync(category_id);
            if (category == null)
            {
                return BadRequest("Category Not Found");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username && task.list_id == category.category_id).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }

            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<CustomTask>>> GetTasksClient()
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("user/date/{date}")]
        public async Task<ActionResult<IEnumerable<CustomTask>>> GetTasksDate(string date)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            if (!DateTime.TryParse(HttpUtility.UrlDecode(date), out var parsedDate))
            {
                return BadRequest("Invalid date format.");
            }

            var tasks = await _context.tasks.Where(task =>
            task.client_user == username && task.task_due_date.HasValue && task.task_due_date.Value.Date == parsedDate.Date).ToListAsync();

            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomTask>> GetTask(int id)
        {
            var task = await _context.tasks.FindAsync(id);
            if (task == null)
            {
                return BadRequest("Task Not Found");
            }
            return Ok(task);
        }

        // HTTP POSTS -------

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CustomTask>> PostTasks(TaskDto task)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }
            var task_db = new CustomTask(
                task.task_name,
                task.task_desc,
                task.list_id,
                task.task_due_date,
                task.state_name,
                username
            );

            _context.tasks.Add(task_db);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TaskExists(task_db.task_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(task);
        }

        // HTTP DELETES -------

        [Authorize]
        [HttpDelete]
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

        // HTTP PUT -------

        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<ActionResult<IEnumerable<CustomTask>>> EditTask(int id, [FromBody] TaskDto edited_Task)
        {
            var task = await _context.tasks.FindAsync(id);
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

            if (edited_Task.list_id != null)
            {
                var list = _context.categories.Where(category => category.category_id == edited_Task.list_id).FirstOrDefault();
                if (list == null)
                {
                    return BadRequest("Category Not Found");
                }
                task.list_id = edited_Task.list_id;
            }
            if (edited_Task.task_due_date != null)
            {
                task.task_due_date = edited_Task.task_due_date;
            }
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        // METHODS -------

        private bool TaskExists(int task_id)
        {
            return _context.tasks.Any(task => task.task_id == task_id);
        }
    }
}
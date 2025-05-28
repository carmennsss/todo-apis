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

        /// <summary>
        /// Gets all the tasks from a specific status
        /// (ex. late, paused...)
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <param name="state_name"></param>
        /// <returns>
        /// Ok status with the tasks or BadRequest or Unauthorized
        /// </returns>
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

        /// <summary>
        /// Gets all the tasks of a specific category
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns>
        /// Ok status with the tasks or BadRequest or Unauthorized
        /// </returns>
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

        /// <summary>
        /// Gets all the tasks from a client
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <returns>
        /// Ok status with the tasks or BadRequest or Unauthorized
        /// </returns>
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

        /// <summary>
        /// Gets all the tasks of a specific date
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <param name="date"></param>
        /// <returns>
        /// Ok status with the tasks or BadRequest or Unauthorized
        /// </returns>
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

        /// <summary>
        /// Gets a specific task based on it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Ok status with the task or BadRequest
        /// </returns>
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

        /// <summary>
        /// Creates a task
        /// First we transform the request data
        /// and then we add it to the db
        /// </summary>
        /// <param name="task"></param>
        /// <returns>
        /// Ok status with the task or Conflict or Unauthorized
        /// </returns>
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

        /// <summary>
        /// Deletes a specific task based on it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// No content status
        /// </returns>
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

        /// <summary>
        /// Gets an id from an already created task 
        /// and a new task, then edits the task from the db
        /// 
        /// Finds the task with it's id and replaces it with the new task
        /// from the request
        /// It's only replaced if the field is not empty or null
        /// The we save the changes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="edited_Task"></param>
        /// <returns>
        /// Ok status with the task or BadRequest
        /// </returns>
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